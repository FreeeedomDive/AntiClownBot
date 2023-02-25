using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class Economies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "EconomiesVersion");

            migrationBuilder.CreateTable(
                name: "Economies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScamCoins = table.Column<int>(type: "integer", nullable: false),
                    NextTribute = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LootBoxes = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXT VALUE FOR EconomiesVersion")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Economies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Economies");

            migrationBuilder.DropSequence(
                name: "EconomiesVersion");
        }
    }
}
