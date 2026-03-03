using AntiClown.DiscordBot.Interactivity.Domain.Shop;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Shops;

public interface IShopEmbedBuilder
{
    Task<DiscordEmbed> BuildAsync(ShopDetails shopDetails);
}