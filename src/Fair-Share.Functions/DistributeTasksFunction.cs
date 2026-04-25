using System;
using System.Configuration;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Fair_Share.Functions;

public class DistributeTasksFunction
{
    private readonly ILogger<DistributeTasksFunction> _logger;

    public DistributeTasksFunction(ILogger<DistributeTasksFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(DistributeTasksFunction))]
    public async Task Run(
        [ServiceBusTrigger("tasksQueue", Connection = "serviceBus")]
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions
    )
    {
        var body = message.Body.ToString();
        var taskMessage = JsonSerializer.Deserialize<TaskMessage>(body);

        var connectionString = Environment.GetEnvironmentVariable(
            "ConnectionStrings__fair-share-db"
        );

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // Get candidates and their ratings
        await using var cmd = new NpgsqlCommand(
            @"
        SELECT 
            a.id AS user_id,
            COALESCE(atp.score, 5) AS rating,
            a.points
        FROM task t
        JOIN account a 
            ON t.team_owned_id = a.team_id
        LEFT JOIN account_task_preference atp
            ON atp.task_id = t.id
            AND atp.account_id = a.id
        WHERE t.id = @taskId;",
            conn
        );

        cmd.Parameters.AddWithValue("@taskId", taskMessage.TaskId);

        // Calculate best candidate based on rating and points
        int bestCandidateId = -1;
        double bestCandidateScore = 0;
        var random = new Random();

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var userId = reader.GetInt32(reader.GetOrdinal("user_id"));
                var rating = reader.GetInt32(reader.GetOrdinal("rating"));
                var points = reader.GetInt32(reader.GetOrdinal("points"));

                var score = Math.Pow(rating, 2) / (points + 1);

                // higher score wins
                if (score > bestCandidateScore)
                {
                    bestCandidateScore = score;
                    bestCandidateId = userId;
                }
                // tie = random pick
                else if (Math.Abs(score - bestCandidateScore) < 0.000001)
                {
                    if (random.Next(0, 2) == 0)
                    {
                        bestCandidateId = userId;
                    }
                }
            }
        }

        if (bestCandidateId == -1)
        {
            _logger.LogWarning("No candidate found for task {TaskId}", taskMessage.TaskId);
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        // Assign task
        await using var assignCmd = new NpgsqlCommand(
            @"
        INSERT INTO account_task (account_id, task_id, assigned_at) VALUES (@accountId, @taskId, @assignedAt);",
            conn
        );
        assignCmd.Parameters.AddWithValue("@accountId", bestCandidateId);
        assignCmd.Parameters.AddWithValue("@taskId", taskMessage.TaskId);
        assignCmd.Parameters.AddWithValue("@assignedAt", DateTime.Now);
        await assignCmd.ExecuteNonQueryAsync();

        // Update points
        await using var updateUserCmd = new NpgsqlCommand(
            @"
        UPDATE account
        SET points = points + @pointsWorth
        WHERE id = @accountId;",
            conn
        );
        updateUserCmd.Parameters.AddWithValue("@accountId", bestCandidateId);
        updateUserCmd.Parameters.AddWithValue("@pointsWorth", taskMessage.PointsWorth);
        await updateUserCmd.ExecuteNonQueryAsync();

        await messageActions.CompleteMessageAsync(message);
    }
}

public class TaskMessage
{
    public int TaskId { get; set; }

    public int PointsWorth { get; set; }
    public DateTime ScheduledTime { get; set; }
}
