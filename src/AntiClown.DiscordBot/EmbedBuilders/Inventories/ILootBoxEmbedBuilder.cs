using AntiClown.Api.Dto.Inventories;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Inventories;

public interface ILootBoxEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(Guid userId, LootBoxRewardDto lootBoxReward);
}