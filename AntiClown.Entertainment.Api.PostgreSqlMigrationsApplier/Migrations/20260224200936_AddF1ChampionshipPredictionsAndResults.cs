#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class AddF1ChampionshipPredictionsAndResults : Migration
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

            migrationBuilder.CreateTable(
                name: "F1ChampionshipResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F1ChampionshipResults", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_F1ChampionshipResults_Season",
                table: "F1ChampionshipResults",
                column: "Season");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F1ChampionshipResults");

            migrationBuilder.DropTable(
                name: "F1ChampionshipPredictions");
        }
    }
}
