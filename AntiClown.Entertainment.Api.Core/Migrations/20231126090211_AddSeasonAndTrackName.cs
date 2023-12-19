using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonAndTrackName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "F1PredictionsRaces",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "F1PredictionsRaces",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_F1PredictionsRaces_Name",
                table: "F1PredictionsRaces",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_F1PredictionsRaces_Season",
                table: "F1PredictionsRaces",
                column: "Season");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_F1PredictionsRaces_Name",
                table: "F1PredictionsRaces");

            migrationBuilder.DropIndex(
                name: "IX_F1PredictionsRaces_Season",
                table: "F1PredictionsRaces");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "F1PredictionsRaces");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "F1PredictionsRaces");
        }
    }
}
