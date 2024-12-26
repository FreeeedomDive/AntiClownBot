using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamsForF1Predictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F1PredictionTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FirstDriver = table.Column<string>(type: "text", nullable: false),
                    SecondDriver = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F1PredictionTeams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F1PredictionTeams_Name",
                table: "F1PredictionTeams",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F1PredictionTeams");
        }
    }
}
