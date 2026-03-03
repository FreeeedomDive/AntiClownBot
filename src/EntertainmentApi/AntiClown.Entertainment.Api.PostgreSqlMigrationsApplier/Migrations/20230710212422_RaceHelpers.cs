#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class RaceHelpers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RaceDrivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverName = table.Column<string>(type: "text", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    CorneringSkill = table.Column<double>(type: "double precision", nullable: false),
                    AccelerationSkill = table.Column<double>(type: "double precision", nullable: false),
                    BreakingSkill = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceDrivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IdealTime = table.Column<int>(type: "integer", nullable: false),
                    CorneringDifficulty = table.Column<int>(type: "integer", nullable: false),
                    AccelerationDifficulty = table.Column<int>(type: "integer", nullable: false),
                    BreakingDifficulty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceTracks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaceDrivers_DriverName",
                table: "RaceDrivers",
                column: "DriverName");

            migrationBuilder.CreateIndex(
                name: "IX_RaceTracks_Name",
                table: "RaceTracks",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaceDrivers");

            migrationBuilder.DropTable(
                name: "RaceTracks");
        }
    }
}
