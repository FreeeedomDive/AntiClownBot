#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class Items : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ItemSpecs = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_OwnerId",
                table: "Items",
                columns: new[] { "OwnerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsActive",
                table: "Items",
                columns: new[] { "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                table: "Items",
                columns: new[] { "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
