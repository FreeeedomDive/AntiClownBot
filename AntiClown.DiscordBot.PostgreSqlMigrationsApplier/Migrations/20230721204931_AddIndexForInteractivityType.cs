#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.DiscordBot.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForInteractivityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Interactivity_Type",
                table: "Interactivity",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Interactivity_Type",
                table: "Interactivity");
        }
    }
}
