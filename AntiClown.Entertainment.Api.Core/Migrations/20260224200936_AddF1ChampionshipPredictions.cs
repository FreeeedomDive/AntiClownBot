using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddF1ChampionshipPredictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F1ChampionshipPredictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    DriverStandings = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F1ChampionshipPredictions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F1ChampionshipPredictions_Season",
                table: "F1ChampionshipPredictions",
                column: "Season");

            migrationBuilder.CreateIndex(
                name: "IX_F1ChampionshipPredictions_UserId",
                table: "F1ChampionshipPredictions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_F1ChampionshipPredictions_Season_UserId",
                table: "F1ChampionshipPredictions",
                columns: new[] { "Season", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F1ChampionshipPredictions");
        }
    }
}
