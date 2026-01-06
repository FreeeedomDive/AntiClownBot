using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverPositionPointsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverPositionPoints",
                table: "F1PredictionsResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverPositionPoints",
                table: "F1PredictionsResults");
        }
    }
}
