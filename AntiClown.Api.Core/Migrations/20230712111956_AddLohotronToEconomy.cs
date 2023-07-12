using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddLohotronToEconomy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLohotronReady",
                table: "Economies",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLohotronReady",
                table: "Economies");
        }
    }
}
