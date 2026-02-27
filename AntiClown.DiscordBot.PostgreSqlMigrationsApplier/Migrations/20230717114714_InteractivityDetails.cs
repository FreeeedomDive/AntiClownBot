#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.DiscordBot.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class InteractivityDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Interactivity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Interactivity");
        }
    }
}
