#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class MinecraftApiMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MinecraftAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    UsernameAndPasswordHash = table.Column<string>(type: "text", nullable: true),
                    AccessTokenHash = table.Column<string>(type: "text", nullable: true),
                    SkinUrl = table.Column<string>(type: "text", nullable: true),
                    CapeUrl = table.Column<string>(type: "text", nullable: true),
                    DiscordId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinecraftAccounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MinecraftAccounts_Username",
                table: "MinecraftAccounts",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinecraftAccounts");
        }
    }
}
