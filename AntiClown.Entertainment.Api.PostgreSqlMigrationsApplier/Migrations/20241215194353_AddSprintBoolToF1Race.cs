#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class AddSprintBoolToF1Race : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSprint",
                table: "F1PredictionsRaces",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSprint",
                table: "F1PredictionsRaces");
        }
    }
}
