using AntiClown.Api.Client;
using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Extensions.Items;
using AntiClown.DiscordBot.Interactivity.Domain.Shop;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Shops;

public class ShopEmbedBuilder : IShopEmbedBuilder
{
    public ShopEmbedBuilder(
        IUsersCache usersCache,
        IEmotesCache emotesCache,
        IAntiClownApiClient antiClownApiClient
    )
    {
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
        this.antiClownApiClient = antiClownApiClient;
    }

    public async Task<DiscordEmbed> BuildAsync(ShopDetails shopDetails)
    {
        var pepegaCredit = await emotesCache.GetEmoteAsTextAsync("PepegaCredit");
        var member = await usersCache.GetMemberByApiIdAsync(shopDetails.UserId);
        var economy = await antiClownApiClient.Economy.ReadAsync(shopDetails.UserId);
        var embedBuilder = new DiscordEmbedBuilder();
        embedBuilder.WithTitle($"Магазин пользователя {member.ServerOrUserName()} {pepegaCredit} {pepegaCredit} {pepegaCredit}");
        embedBuilder.AddField("Баланс", $"{economy.ScamCoins}", true);
        embedBuilder.AddField("Цена реролла магазина", $"{shopDetails.Shop.ReRollPrice}", true);
        embedBuilder.AddField("Распознавание предмета", $"{shopDetails.Shop.FreeReveals}", true);
        var itemIndex = 1;
        var maxRarity = shopDetails.Shop.Items.OrderByDescending(item => item.Rarity).First().Rarity;
        embedBuilder.WithColor(Color[maxRarity]);
        foreach (var shopItem in shopDetails.Shop.Items)
        {
            var boughtItem = shopItem.IsOwned && shopDetails.BoughtItems.TryGetValue(itemIndex - 1, out var itemId)
                ? await antiClownApiClient.Inventories.ReadItemAsync(shopDetails.UserId, itemId)
                : null;
            var itemContent = boughtItem is not null
                ? $"КУПЛЕН\n{boughtItem.Description().ToStatsString()}"
                : shopItem.IsOwned
                    ? "КУПЛЕН"
                    : $"Редкость: {Rarity[shopItem.Rarity]}\n" +
                      $"Цена: {shopItem.Price}";
            embedBuilder.AddField(
                $"{itemIndex}. " + (shopItem.IsRevealed ? shopItem.Name.Localize() :
                    shopItem.IsOwned ? shopItem.Name.Localize() : "Нераспознанный предмет"),
                itemContent
            );
            itemIndex++;
        }

        embedBuilder.WithFooter($"Текущее действие: {StringTool(shopDetails.Tool)}");

        return embedBuilder.Build();
    }

    private static string StringTool(ShopTool tool)
    {
        return tool switch
        {
            ShopTool.Buying => "покупка",
            ShopTool.Revealing => "распознавание",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IEmotesCache emotesCache;

    private readonly IUsersCache usersCache;

    private static readonly Dictionary<RarityDto, string> Rarity = new()
    {
        { RarityDto.Common, "Обычная" },
        { RarityDto.Rare, "Редкая" },
        { RarityDto.Epic, "Эпическая" },
        { RarityDto.Legendary, "Легендарная" },
        { RarityDto.BlackMarket, "С черного рынка" },
    };

    private static readonly Dictionary<RarityDto, DiscordColor> Color = new()
    {
        { RarityDto.Common, DiscordColor.Gray },
        { RarityDto.Rare, DiscordColor.Blue },
        { RarityDto.Epic, DiscordColor.Purple },
        { RarityDto.Legendary, DiscordColor.Red },
        { RarityDto.BlackMarket, DiscordColor.Black },
    };
}