using AntiClown.Api.Dto.Economies;
using AntiClown.Api.Dto.Inventories;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Rating;

public interface IRatingEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(EconomyDto economy, InventoryDto inventory);
}