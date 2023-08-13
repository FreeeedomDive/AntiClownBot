using AntiClown.Api.Dto.Economies;
using AntiClown.Api.Dto.Extensions;
using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Rating;

public class RatingEmbedBuilder : IRatingEmbedBuilder
{
    public RatingEmbedBuilder(
        IUsersCache usersCache,
        IEmotesCache emotesCache
    )
    {
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
    }

    public async Task<DiscordEmbed> BuildAsync(EconomyDto economy, InventoryDto inventory)
    {
        var member = await usersCache.GetMemberByApiIdAsync(economy.Id);
        var embedBuilder = new DiscordEmbedBuilder
        {
            Color = member!.Color,
        };
        var name = member.ServerOrUserName();
        embedBuilder.WithThumbnail(member.AvatarUrl);
        var aRolf = await emotesCache.GetEmoteAsTextAsync("aRolf");
        embedBuilder.WithTitle($"ЧЕЛА РЕАЛЬНО ЗОВУТ {name.ToUpper()} {aRolf} {aRolf} {aRolf}");

        embedBuilder.AddField("Скам-койны", $"{economy.ScamCoins}");
        embedBuilder.AddField("Общая ценность", $"{inventory.NetWorth}");

        AddFieldForItems(embedBuilder, inventory.CatWives, ItemNameDto.CatWife);
        AddFieldForItems(embedBuilder, inventory.DogWives, ItemNameDto.DogWife);
        AddFieldForItems(embedBuilder, inventory.Internets, ItemNameDto.Internet);
        AddFieldForItems(embedBuilder, inventory.RiceBowls, ItemNameDto.RiceBowl);
        AddFieldForItems(embedBuilder, inventory.CommunismBanners, ItemNameDto.CommunismBanner);
        AddFieldForItems(embedBuilder, inventory.JadeRods, ItemNameDto.JadeRod);

        embedBuilder.AddField($"Добыча-коробка - {economy.LootBoxes}", "Получение приза из лутбокса");

        return embedBuilder.Build();
    }

    private static void AddFieldForItems<T>(DiscordEmbedBuilder embedBuilder, T[] items, ItemNameDto itemName) where T : BaseItemDto
    {
        var onlyActiveItems = items.Where(x => x.IsActive).ToArray();
        var descriptions = onlyActiveItems.Length == 0
            ? "Нет предметов"
            : $"{string.Join(" ", onlyActiveItems.Select(item => $"{item.Rarity}"))}\n" +
              $"{string.Join("\n", CalculateItemStats(onlyActiveItems, itemName).Select(kv => $"{kv.Key}: {kv.Value}"))}";
        embedBuilder.AddField(
            $"{itemName.Localize()} - {onlyActiveItems.Length} (всего {items.Length})",
            descriptions
        );
    }

    private static Dictionary<string, string> CalculateItemStats<T>(T[] items, ItemNameDto itemName) where T : BaseItemDto
    {
        return itemName switch
        {
            ItemNameDto.CatWife => new Dictionary<string, string>
            {
                {
                    "Шанс на автоматическое подношение",
                    $"{items.CatWives().Select(i => i.AutoTributeChance).Sum()}%"
                },
            },
            ItemNameDto.CommunismBanner => new Dictionary<string, string>
            {
                {
                    "Шанс разделить награду за подношение с другим владельцем плаката",
                    $"{items.CommunismBanners().Select(i => i.DivideChance).Sum()}%"
                },
                {
                    "Приоритет стащить чужое подношение (если у него сработал плакат)",
                    $"{items.CommunismBanners().Select(i => i.StealChance).Sum()}"
                },
            },
            ItemNameDto.DogWife => new Dictionary<string, string>
            {
                {
                    "Шанс получить лутбокс во время подношения",
                    $"{(double)items.DogWives().Select(i => i.LootBoxFindChance).Sum() / 10}%"
                },
            },
            ItemNameDto.Internet => new Dictionary<string, string>
            {
                {
                    "Уменьшение кулдауна в процентах",
                    $"{string.Join("    ", items.Internets().Select(i => $"{i.Speed}%"))}"
                },
                {
                    "Общее количество попыток уменьшить кулдаун",
                    $"{items.Internets().Select(i => i.Gigabytes).Sum()}"
                },
                {
                    "Шанс уменьшения кулдауна во время одной попытки",
                    $"{string.Join("    ", items.Internets().Select(i => $"{i.Ping}%"))}"
                },
            },
            ItemNameDto.JadeRod => new Dictionary<string, string>
            {
                {
                    "Шанс увеличения кулдауна во время одной попытки",
                    $"{TributeHelpers.CooldownIncreaseChanceByOneJade}%"
                },
                {
                    "Общее количество попыток увеличить кулдаун",
                    $"{items.JadeRods().Select(i => i.Length).Sum()}"
                },
                {
                    "Увеличение кулдауна в процентах",
                    $"{string.Join("\t", items.JadeRods().Select(i => $"{i.Thickness}%"))}"
                },
            },
            ItemNameDto.RiceBowl => new Dictionary<string, string>
            {
                {
                    "Границы получения подношения",
                    $"от {TributeHelpers.MinTributeValue - items.RiceBowls().Select(i => i.NegativeRangeExtend).Sum()}"
                    + $" до {TributeHelpers.MaxTributeValue + items.RiceBowls().Select(i => i.PositiveRangeExtend).Sum()}"
                },
            },
            _ => throw new ArgumentOutOfRangeException(nameof(itemName), itemName, null),
        };
    }

    private readonly IEmotesCache emotesCache;
    private readonly IUsersCache usersCache;
}