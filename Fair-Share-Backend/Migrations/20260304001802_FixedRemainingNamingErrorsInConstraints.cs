using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fair_Share_Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixedRemainingNamingErrorsInConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "member_task_member_id_fkey",
                table: "account_task");

            migrationBuilder.DropForeignKey(
                name: "member_task_task_id_fkey",
                table: "account_task");

            migrationBuilder.DropTable(
                name: "AccountTeam");

            migrationBuilder.DropTable(
                name: "team_member");

            migrationBuilder.DropPrimaryKey(
                name: "member_task_pkey",
                table: "account_task");

            migrationBuilder.DropPrimaryKey(
                name: "member_pkey",
                table: "account");

            migrationBuilder.AddPrimaryKey(
                name: "account_task_pkey",
                table: "account_task",
                columns: new[] { "account_id", "task_id" });

            migrationBuilder.AddPrimaryKey(
                name: "account_pkey",
                table: "account",
                column: "id");

            migrationBuilder.CreateTable(
                name: "team_account",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("team_account_pkey", x => new { x.account_id, x.team_id });
                    table.ForeignKey(
                        name: "team_account_account_id_fkey",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "team_account_team_id_fkey",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_team_account_team_id",
                table: "team_account",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "account_task_account_id_fkey",
                table: "account_task",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "account_task_task_id_fkey",
                table: "account_task",
                column: "task_id",
                principalTable: "task",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "account_task_account_id_fkey",
                table: "account_task");

            migrationBuilder.DropForeignKey(
                name: "account_task_task_id_fkey",
                table: "account_task");

            migrationBuilder.DropTable(
                name: "team_account");

            migrationBuilder.DropPrimaryKey(
                name: "account_task_pkey",
                table: "account_task");

            migrationBuilder.DropPrimaryKey(
                name: "account_pkey",
                table: "account");

            migrationBuilder.AddPrimaryKey(
                name: "member_task_pkey",
                table: "account_task",
                columns: new[] { "account_id", "task_id" });

            migrationBuilder.AddPrimaryKey(
                name: "member_pkey",
                table: "account",
                column: "id");

            migrationBuilder.CreateTable(
                name: "AccountTeam",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTeam", x => new { x.AccountId, x.TeamId });
                });

            migrationBuilder.CreateTable(
                name: "team_member",
                columns: table => new
                {
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("team_member_pkey", x => new { x.member_id, x.team_id });
                    table.ForeignKey(
                        name: "team_member_member_id_fkey",
                        column: x => x.member_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "team_member_team_id_fkey",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_team_member_team_id",
                table: "team_member",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "member_task_member_id_fkey",
                table: "account_task",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "member_task_task_id_fkey",
                table: "account_task",
                column: "task_id",
                principalTable: "task",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
