using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Interactivity.Domain.Inventory;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Inventories;

public interface IInventoryEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(InventoryDetails inventoryDetails, IEnumerable<BaseItemDto> items);
}