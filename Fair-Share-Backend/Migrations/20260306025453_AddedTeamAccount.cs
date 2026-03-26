using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fair_Share_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedTeamAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "points",
                table: "task",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "points",
                table: "account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "account_task_preference",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    task_id = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_task_preference", x => new { x.account_id, x.task_id });
                    table.ForeignKey(
                        name: "FK_account_task_preference_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_task_preference_task_task_id",
                        column: x => x.task_id,
                        principalTable: "task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_task_preference_task_id",
                table: "account_task_preference",
                column: "task_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_task_preference");

            migrationBuilder.DropColumn(
                name: "points",
                table: "task");

            migrationBuilder.DropColumn(
                name: "points",
                table: "account");
        }
    }
}
