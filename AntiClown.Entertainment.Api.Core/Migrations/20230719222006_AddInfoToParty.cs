using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntiClown.Entertainment.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddInfoToParty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Parties",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstFullPartyAt",
                table: "Parties",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_CreatorId",
                table: "Parties",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_FirstFullPartyAt",
                table: "Parties",
                column: "FirstFullPartyAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parties_CreatorId",
                table: "Parties");

            migrationBuilder.DropIndex(
                name: "IX_Parties_FirstFullPartyAt",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "FirstFullPartyAt",
                table: "Parties");
        }
    }
}
