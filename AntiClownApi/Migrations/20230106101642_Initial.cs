using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AntiClownBotApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emotes",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    Rarity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEconomies",
                columns: table => new
                {
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ScamCoins = table.Column<int>(type: "integer", nullable: false),
                    NextTribute = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LootBoxes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEconomies", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserEconomies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserShops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ReRollPrice = table.Column<int>(type: "integer", nullable: false),
                    FreeItemReveals = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserShops_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStats",
                columns: table => new
                {
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TributeCount = table.Column<int>(type: "integer", nullable: false),
                    WrittenMessagesCount = table.Column<int>(type: "integer", nullable: false),
                    JoinedPartyCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStats", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserStats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CatAutoTributeChance = table.Column<int>(type: "integer", nullable: false),
                    CommunismDivideChance = table.Column<int>(type: "integer", nullable: false),
                    CommunismStealChance = table.Column<int>(type: "integer", nullable: false),
                    DogLootBoxFindChance = table.Column<int>(type: "integer", nullable: false),
                    InternetSpeed = table.Column<int>(type: "integer", nullable: false),
                    InternetGigabytes = table.Column<int>(type: "integer", nullable: false),
                    InternetPing = table.Column<int>(type: "integer", nullable: false),
                    JadeRodThickness = table.Column<int>(type: "integer", nullable: false),
                    JadeRodLength = table.Column<int>(type: "integer", nullable: false),
                    RiceNegativeRangeExtend = table.Column<int>(type: "integer", nullable: false),
                    RicePositiveRangeExtend = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStats_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserEconomyId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RatingChange = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_UserEconomies_UserEconomyId",
                        column: x => x.UserEconomyId,
                        principalTable: "UserEconomies",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShopId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Rarity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    IsRevealed = table.Column<bool>(type: "boolean", nullable: false),
                    IsOwned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopItems_UserShops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "UserShops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEmotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatsId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    EmoteId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEmotes_Emotes_StatsId",
                        column: x => x.StatsId,
                        principalTable: "Emotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEmotes_UserStats_StatsId",
                        column: x => x.StatsId,
                        principalTable: "UserStats",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemStats_ItemId",
                table: "ItemStats",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_ShopId",
                table: "ShopItems",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserEconomyId",
                table: "Transactions",
                column: "UserEconomyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmotes_StatsId",
                table: "UserEmotes",
                column: "StatsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShops_UserId",
                table: "UserShops",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemStats");

            migrationBuilder.DropTable(
                name: "ShopItems");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserEmotes");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "UserShops");

            migrationBuilder.DropTable(
                name: "UserEconomies");

            migrationBuilder.DropTable(
                name: "Emotes");

            migrationBuilder.DropTable(
                name: "UserStats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
