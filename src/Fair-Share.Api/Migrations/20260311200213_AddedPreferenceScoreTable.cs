using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fair_Share.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedPreferenceScoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_task_preference_account_account_id",
                table: "account_task_preference");

            migrationBuilder.DropForeignKey(
                name: "FK_account_task_preference_task_task_id",
                table: "account_task_preference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_account_task_preference",
                table: "account_task_preference");

            migrationBuilder.AddPrimaryKey(
                name: "account_task_preference_pkey",
                table: "account_task_preference",
                columns: new[] { "account_id", "task_id" });

            migrationBuilder.AddForeignKey(
                name: "account_task_preference_account_id_fkey",
                table: "account_task_preference",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "account_task_preference_task_id_fkey",
                table: "account_task_preference",
                column: "task_id",
                principalTable: "task",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "account_task_preference_account_id_fkey",
                table: "account_task_preference");

            migrationBuilder.DropForeignKey(
                name: "account_task_preference_task_id_fkey",
                table: "account_task_preference");

            migrationBuilder.DropPrimaryKey(
                name: "account_task_preference_pkey",
                table: "account_task_preference");

            migrationBuilder.AddPrimaryKey(
                name: "PK_account_task_preference",
                table: "account_task_preference",
                columns: new[] { "account_id", "task_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_account_task_preference_account_account_id",
                table: "account_task_preference",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_account_task_preference_task_task_id",
                table: "account_task_preference",
                column: "task_id",
                principalTable: "task",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
