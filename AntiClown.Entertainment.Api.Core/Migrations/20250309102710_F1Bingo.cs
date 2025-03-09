using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class F1Bingo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F1BingoBoards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Cards = table.Column<Guid[]>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F1BingoBoards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "F1BingoCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: true),
                    Probability = table.Column<string>(type: "text", nullable: false),
                    TotalRepeats = table.Column<int>(type: "integer", nullable: false),
                    CompletedRepeats = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F1BingoCards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F1BingoBoards_UserId_Season",
                table: "F1BingoBoards",
                columns: new[] { "UserId", "Season" });

            migrationBuilder.CreateIndex(
                name: "IX_F1BingoCards_Season",
                table: "F1BingoCards",
                column: "Season");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F1BingoBoards");

            migrationBuilder.DropTable(
                name: "F1BingoCards");
        }
    }
}
