using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fair_Share_Backend.Migrations
{
    /// <inheritdoc />
    public partial class TrackedTaskInIndividualTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "team_owned_id",
                table: "task",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_task_team_owned_id",
                table: "task",
                column: "team_owned_id");

            migrationBuilder.AddForeignKey(
                name: "task_team_owned_id_fkey",
                table: "task",
                column: "team_owned_id",
                principalTable: "team",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "task_team_owned_id_fkey",
                table: "task");

            migrationBuilder.DropIndex(
                name: "IX_task_team_owned_id",
                table: "task");

            migrationBuilder.DropColumn(
                name: "team_owned_id",
                table: "task");
        }
    }
}
