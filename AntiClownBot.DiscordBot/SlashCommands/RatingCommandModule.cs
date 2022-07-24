using AntiClownApiClient;
using AntiClownApiClient.Dto.Constants;
using AntiClownApiClient.Dto.Models.Items;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands;

public class RatingCommandModule : ApplicationCommandModule
{
    public RatingCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
    }

    [SlashCommand("scamCoins", "Узнать свой баланс скамкойнов (остальные его не увидят)")]
    public async Task FetchScamCoins(InteractionContext context)
    {
        var userId = context.Member.Id;
        var userRating = await apiClient.Users.RatingAsync(userId);

        await discordClientWrapper.Messages.RespondAsync(
            context,
            $"{userRating.ScamCoins} скам-койнов",
            InteractionResponseType.DeferredChannelMessageWithSource,
            isEphemeral: true
        );
    }

    [SlashCommand("rating", "Полный паспорт пользователя с информацией о балансе и активных предметах")]
    public async Task Rating(InteractionContext context)
    {
        var userId = context.Member.Id;
        var member = await discordClientWrapper.Members.GetAsync(userId);
        var response = await apiClient.Users.RatingAsync(userId);
        var inventory = await apiClient.Items.AllItemsAsync(userId);

        var embedBuilder = new DiscordEmbedBuilder
        {
            Color = member.Color
        };

        var name = member.ServerOrUserName();
        embedBuilder.WithThumbnail(context.Member.AvatarUrl);
        var aRolf = await discordClientWrapper.Emotes.FindEmoteAsync("aRolf");
        embedBuilder.WithTitle($"ЧЕЛА РЕАЛЬНО ЗОВУТ {name.ToUpper()} {aRolf} {aRolf} {aRolf}");

        embedBuilder.AddField("SCAM COINS", $"{response.ScamCoins}");
        embedBuilder.AddField("Общая ценность", $"{response.NetWorth}");

        foreach (var itemName in StringConstants.AllItemsNames)
        {
            var itemsOfType = response.Inventory.Where(item => item.Name.Equals(itemName)).ToList();
            var descriptions = itemsOfType.Count == 0
                ? "Нет предметов"
                : $"{string.Join(" ", itemsOfType.Select(item => $"{item.Rarity}"))}\n" +
                  $"{string.Join("\n", CalculateItemStats(itemsOfType, itemName).Select(kv => $"{kv.Key}: {kv.Value}"))}";
            embedBuilder.AddField(
                $"{itemName} - {itemsOfType.Count} (всего {inventory.Count(item => item.Name == itemName)})",
                descriptions);
        }

        embedBuilder.AddField($"Добыча-коробка - {response.LootBoxes}", "Получение приза из лутбокса");

        await discordClientWrapper.Messages.RespondAsync(context, embedBuilder.Build());
    }

    private static Dictionary<string, string> CalculateItemStats(List<BaseItem> items, string itemType)
    {
        return itemType switch
        {
            StringConstants.CatWifeName => new Dictionary<string, string>()
            {
                {
                    "Шанс на автоматическое подношение",
                    $"{items.Where(i => i.Name == itemType).Select(i => (CatWife)i).Select(i => i.AutoTributeChance).Sum()}%"
                }
            },
            StringConstants.DogWifeName => new Dictionary<string, string>()
            {
                {
                    "Шанс получить лутбокс во время подношения",
                    $"{(double)items.Where(i => i.Name == itemType).Select(i => (DogWife)i).Select(i => i.LootBoxFindChance).Sum() / 10}%"
                }
            },
            StringConstants.RiceBowlName => new Dictionary<string, string>()
            {
                {
                    "Границы получения подношения",
                    $"от {NumericConstants.MinTributeValue - items.Where(i => i.Name == itemType).Select(i => (RiceBowl)i).Select(i => i.NegativeRangeExtend).Sum()}"
                    + $" до {NumericConstants.MaxTributeValue + items.Where(i => i.Name == itemType).Select(i => (RiceBowl)i).Select(i => i.PositiveRangeExtend).Sum()}"
                }
            },
            StringConstants.InternetName => new Dictionary<string, string>()
            {
                {
                    "Уменьшение кулдауна в процентах",
                    $"{string.Join("\t", items.Where(i => i.Name == itemType).Select(i => (Internet)i).Select(i => $"{i.Speed}%"))}"
                },
                {
                    "Общее количество попыток уменьшить кулдаун",
                    $"{items.Where(i => i.Name == itemType).Select(i => (Internet)i).Select(i => i.Gigabytes).Sum()}"
                },
                {
                    "Шанс уменьшения кулдауна во время одной попытки",
                    $"{string.Join("\t", items.Where(i => i.Name == itemType).Select(i => (Internet)i).Select(i => $"{i.Ping}%"))}"
                }
            },
            StringConstants.JadeRodName => new Dictionary<string, string>()
            {
                {
                    "Шанс увеличения кулдауна во время одной попытки",
                    $"{NumericConstants.CooldownIncreaseChanceByOneJade}%"
                },
                {
                    "Общее количество попыток увеличить кулдаун",
                    $"{items.Where(i => i.Name == itemType).Select(i => (JadeRod)i).Select(i => i.Length).Sum()}"
                },
                {
                    "Увеличение кулдауна в процентах",
                    $"{string.Join("\t", items.Where(i => i.Name == itemType).Select(i => (JadeRod)i).Select(i => $"{i.Thickness}%"))}"
                }
            },
            StringConstants.CommunismBannerName => new Dictionary<string, string>()
            {
                {
                    "Шанс разделить награду за подношение с другим владельцем плаката",
                    $"{items.Where(i => i.Name == itemType).Select(i => (CommunismBanner)i).Select(i => i.DivideChance).Sum()}%"
                },
                {
                    "Приоритет стащить чужое подношение (если у него сработал плакат)",
                    $"{items.Where(i => i.Name == itemType).Select(i => (CommunismBanner)i).Select(i => i.StealChance).Sum()}"
                }
            },

            _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
        };
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
}