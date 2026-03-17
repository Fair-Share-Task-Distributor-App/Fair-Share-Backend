using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fair_Share_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelationshipBetweenAccountAndTeamToBeOnetoManyByRemovingAccountTeamTable
        : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "team_account");

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "account",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_account_team_id",
                table: "account",
                column: "team_id"
            );

            migrationBuilder.AddForeignKey(
                name: "account_team_id_fkey",
                table: "account",
                column: "team_id",
                principalTable: "team",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "account_team_id_fkey", table: "account");

            migrationBuilder.DropIndex(name: "IX_account_team_id", table: "account");

            migrationBuilder.DropColumn(name: "team_id", table: "account");

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
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "team_account_team_id_fkey",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_team_account_team_id",
                table: "team_account",
                column: "team_id"
            );
        }
    }
}
