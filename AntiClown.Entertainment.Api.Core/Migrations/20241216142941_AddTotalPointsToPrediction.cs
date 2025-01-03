using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPointsToPrediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "F1PredictionsResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "F1PredictionsResults");
        }
    }
}
