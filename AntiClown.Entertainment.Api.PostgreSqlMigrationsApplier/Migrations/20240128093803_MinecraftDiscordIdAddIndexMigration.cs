#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class MinecraftDiscordIdAddIndexMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MinecraftAccounts_DiscordId",
                table: "MinecraftAccounts",
                column: "DiscordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MinecraftAccounts_DiscordId",
                table: "MinecraftAccounts");
        }
    }
}
