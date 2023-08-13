using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class F1Predictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F1PredictionsRaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsOpened = table.Column<bool>(type: "boolean", nullable: false),
                    SerializedPredictions = table.Column<string>(type: "text", nullable: false),
                    SerializedResults = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F1PredictionsRaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "F1PredictionsResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenthPlacePoints = table.Column<int>(type: "integer", nullable: false),
                    FirstDnfPoints = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F1PredictionsResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F1PredictionsRaces_IsActive",
                table: "F1PredictionsRaces",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_F1PredictionsResults_RaceId",
                table: "F1PredictionsResults",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_F1PredictionsResults_UserId",
                table: "F1PredictionsResults",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F1PredictionsRaces");

            migrationBuilder.DropTable(
                name: "F1PredictionsResults");
        }
    }
}
