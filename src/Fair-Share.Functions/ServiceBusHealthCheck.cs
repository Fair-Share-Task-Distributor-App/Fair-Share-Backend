using System.Net;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public class ServiceBusHealthCheck
{
    private readonly ILogger _logger;

    public ServiceBusHealthCheck(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ServiceBusHealthCheck>();
    }

    [Function("DB_CHECK")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req
    )
    {
        string? connectionString = "";
        if (string.IsNullOrEmpty(connectionString))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("ServiceBusConnection is missing in configuration.");
            return badResponse;
        }

        try
        {
            await using var client = new ServiceBusClient(connectionString);

            string queueName = "tasksQueue";

            ServiceBusSender sender = client.CreateSender(queueName);

            ServiceBusMessage message = new("Health check message");

            await sender.SendMessageAsync(message);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Service Bus connection successful. Message sent.");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Service Bus connection failed.");

            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync("Service Bus connection failed.");

            return response;
        }
    }

    //[Function("ServiceBusHealthCheck")]
    //public async Task<HttpResponseData> Run(
    //    [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req
    //)
    //{
    //    string? connectionString =
    //        "Endpoint=sb://localhost:52475;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

    //    if (string.IsNullOrEmpty(connectionString))
    //    {
    //        var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
    //        await badResponse.WriteStringAsync("ServiceBusConnection is missing in configuration.");
    //        return badResponse;
    //    }

    //    try
    //    {
    //        await using var client = new ServiceBusClient(connectionString);

    //        string queueName = "tasksQueue";

    //        ServiceBusSender sender = client.CreateSender(queueName);

    //        ServiceBusMessage message = new("Health check message");

    //        await sender.SendMessageAsync(message);

    //        var response = req.CreateResponse(HttpStatusCode.OK);
    //        await response.WriteStringAsync("Service Bus connection successful. Message sent.");

    //        return response;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Service Bus connection failed.");

    //        var response = req.CreateResponse(HttpStatusCode.InternalServerError);
    //        await response.WriteStringAsync("Service Bus connection failed.");

    //        return response;
    //    }
    //}
}
