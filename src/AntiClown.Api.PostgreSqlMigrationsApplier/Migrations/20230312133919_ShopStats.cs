#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class ShopStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalReRolls = table.Column<int>(type: "integer", nullable: false),
                    ItemsBought = table.Column<int>(type: "integer", nullable: false),
                    TotalReveals = table.Column<int>(type: "integer", nullable: false),
                    ScamCoinsLostOnReveals = table.Column<int>(type: "integer", nullable: false),
                    ScamCoinsLostOnReRolls = table.Column<int>(type: "integer", nullable: false),
                    ScamCoinsLostOnPurchases = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopStats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopStats");
        }
    }
}
