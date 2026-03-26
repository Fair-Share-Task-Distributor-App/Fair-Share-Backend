using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fair_Share_Backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameMemberTableToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "team_member_member_id_fkey",
                table: "team_member");

            migrationBuilder.DropTable(
                name: "member_task");

            migrationBuilder.DropTable(
                name: "MemberTeam");

            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "team_member",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("member_pkey", x => x.id);
                });

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
                name: "account_task",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "integer", nullable: false),
                    task_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("member_task_pkey", x => new { x.account_id, x.task_id });
                    table.ForeignKey(
                        name: "member_task_member_id_fkey",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "member_task_task_id_fkey",
                        column: x => x.task_id,
                        principalTable: "task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_task_task_id",
                table: "account_task",
                column: "task_id");

            migrationBuilder.AddForeignKey(
                name: "team_member_member_id_fkey",
                table: "team_member",
                column: "member_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "team_member_member_id_fkey",
                table: "team_member");

            migrationBuilder.DropTable(
                name: "account_task");

            migrationBuilder.DropTable(
                name: "AccountTeam");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "team_member");

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("member_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MemberTeam",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTeam", x => new { x.MemberId, x.TeamId });
                });

            migrationBuilder.CreateTable(
                name: "member_task",
                columns: table => new
                {
                    member_id = table.Column<int>(type: "integer", nullable: false),
                    task_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("member_task_pkey", x => new { x.member_id, x.task_id });
                    table.ForeignKey(
                        name: "member_task_member_id_fkey",
                        column: x => x.member_id,
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "member_task_task_id_fkey",
                        column: x => x.task_id,
                        principalTable: "task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_task_task_id",
                table: "member_task",
                column: "task_id");

            migrationBuilder.AddForeignKey(
                name: "team_member_member_id_fkey",
                table: "team_member",
                column: "member_id",
                principalTable: "member",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
