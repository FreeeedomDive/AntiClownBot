using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.Api.Dto.Extensions;
using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.SocialRating;

public class RatingCommandModule : SlashCommandModuleWithMiddlewares
{
    public RatingCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        IEmotesCache emotesCache,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.emotesCache = emotesCache;
        this.usersCache = usersCache;
        this.antiClownApiClient = antiClownApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.ScamCoins, "Узнать свой баланс скамкойнов (остальные его не увидят)")]
    public async Task FetchScamCoins(InteractionContext context)
    {
        await ExecuteEphemeralAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var userRating = await antiClownApiClient.Economy.ReadAsync(userId);

                await RespondToInteractionAsync(
                    context,
                    $"{userRating.ScamCoins} скам-койнов"
                );
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Rating, "Полный паспорт пользователя с информацией о балансе и активных предметах")]
    public async Task Rating(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var economy = await antiClownApiClient.Economy.ReadAsync(userId);
                var inventory = await antiClownApiClient.Inventories.ReadInventoryAsync(userId);

                var embedBuilder = new DiscordEmbedBuilder
                {
                    Color = context.Member.Color,
                };

                var name = context.Member.ServerOrUserName();
                embedBuilder.WithThumbnail(context.Member.AvatarUrl);
                var aRolf = await emotesCache.GetEmoteAsTextAsync("aRolf");
                embedBuilder.WithTitle($"ЧЕЛА РЕАЛЬНО ЗОВУТ {name.ToUpper()} {aRolf} {aRolf} {aRolf}");

                embedBuilder.AddField("SCAM COINS", $"{economy.ScamCoins}");
                // TODO: embedBuilder.AddField("Общая ценность", $"{response.NetWorth}");

                AddFieldForItems(embedBuilder, inventory.CatWives, ItemNameDto.CatWife);
                AddFieldForItems(embedBuilder, inventory.DogWives, ItemNameDto.DogWife);
                AddFieldForItems(embedBuilder, inventory.Internets, ItemNameDto.Internet);
                AddFieldForItems(embedBuilder, inventory.RiceBowls, ItemNameDto.RiceBowl);
                AddFieldForItems(embedBuilder, inventory.CommunismBanners, ItemNameDto.CommunismBanner);
                AddFieldForItems(embedBuilder, inventory.JadeRods, ItemNameDto.JadeRod);

                embedBuilder.AddField($"Добыча-коробка - {economy.LootBoxes}", "Получение приза из лутбокса");

                await RespondToInteractionAsync(context, embedBuilder.Build());
            }
        );
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

    private readonly IAntiClownApiClient antiClownApiClient;

    private readonly IEmotesCache emotesCache;
    private readonly IUsersCache usersCache;
}