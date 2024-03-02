using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class NewPredictionsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DnfPoints",
                table: "F1PredictionsResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FirstPlaceLeadPoints",
                table: "F1PredictionsResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SafetyCarsPoints",
                table: "F1PredictionsResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamMatesPoints",
                table: "F1PredictionsResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DnfPoints",
                table: "F1PredictionsResults");

            migrationBuilder.DropColumn(
                name: "FirstPlaceLeadPoints",
                table: "F1PredictionsResults");

            migrationBuilder.DropColumn(
                name: "SafetyCarsPoints",
                table: "F1PredictionsResults");

            migrationBuilder.DropColumn(
                name: "TeamMatesPoints",
                table: "F1PredictionsResults");
        }
    }
}
