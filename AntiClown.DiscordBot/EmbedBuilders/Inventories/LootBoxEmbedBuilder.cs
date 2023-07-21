using AntiClown.Api.Client;
using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Extensions.Items;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Inventories;

public class LootBoxEmbedBuilder : ILootBoxEmbedBuilder
{
    public LootBoxEmbedBuilder(
        IUsersCache usersCache,
        IAntiClownApiClient antiClownApiClient
    )
    {
        this.usersCache = usersCache;
        this.antiClownApiClient = antiClownApiClient;
    }

    public async Task<DiscordEmbed> BuildAsync(Guid userId, LootBoxRewardDto lootBoxReward)
    {
        var member = await usersCache.GetMemberByApiIdAsync(userId);
        var embedBuilder = new DiscordEmbedBuilder();
        embedBuilder.WithTitle($"{member.ServerOrUserName()} открывает лутбокс...");
        embedBuilder.WithColor(member!.Color);

        var reward = lootBoxReward.ScamCoinsReward;
        embedBuilder.AddField("Денежное вознаграждение", $"Ты получил {reward} scam coins!");

        for (var i = 0; i < lootBoxReward.Items.Length; i++)
        {
            var item = await antiClownApiClient.Inventories.ReadItemAsync(userId, lootBoxReward.Items[i]);
            embedBuilder.AddField(
                $"{i + 1} предмет", $"Ты получил {item.Rarity} {item.ItemName.Localize()}\n" +
                                    string.Join(
                                        "\n",
                                        item.Description()
                                            .Select(kv => $"{kv.Key}: {kv.Value}")
                                    )
            );
        }

        return embedBuilder.Build();
    }

    private readonly IAntiClownApiClient antiClownApiClient;

    private readonly IUsersCache usersCache;
}