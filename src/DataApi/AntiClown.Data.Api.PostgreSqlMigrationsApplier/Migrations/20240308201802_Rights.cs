#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AntiClown.Data.Api.PostgreSqlMigrationsApplier.Migrations
{
    /// <inheritdoc />
    public partial class Rights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rights", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rights_Name",
                table: "Rights",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Rights_UserId",
                table: "Rights",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rights");
        }
    }
}
