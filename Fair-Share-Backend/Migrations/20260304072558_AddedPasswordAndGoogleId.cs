using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fair_Share_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordAndGoogleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "google_id",
                table: "account",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "account",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "account",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "google_id",
                table: "account");

            migrationBuilder.DropColumn(
                name: "password",
                table: "account");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "account");
        }
    }
}
