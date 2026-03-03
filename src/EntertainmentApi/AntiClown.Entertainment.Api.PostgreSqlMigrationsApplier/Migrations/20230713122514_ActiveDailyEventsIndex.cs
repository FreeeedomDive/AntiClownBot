#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class ActiveDailyEventsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveDailyEventsIndex",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveDailyEventsIndex", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveDailyEventsIndex_EventType",
                table: "ActiveDailyEventsIndex",
                column: "EventType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveDailyEventsIndex");
        }
    }
}
