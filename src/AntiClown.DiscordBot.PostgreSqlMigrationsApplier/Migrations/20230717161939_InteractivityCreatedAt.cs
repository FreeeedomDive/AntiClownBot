#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.DiscordBot.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class InteractivityCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Interactivity",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Interactivity");
        }
    }
}
