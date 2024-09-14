using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TelegramId",
                table: "Users",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramId",
                table: "Users");
        }
    }
}
