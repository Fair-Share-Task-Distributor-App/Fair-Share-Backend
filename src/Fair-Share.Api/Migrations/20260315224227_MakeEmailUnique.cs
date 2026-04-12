using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fair_Share.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmailUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "account_email_key",
                table: "account",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "account_email_key",
                table: "account");
        }
    }
}
