using AntiClown.DiscordBot.Interactivity.Domain.Shop;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.Interactivity.Services.Shop;

public interface IShopService
{
    Task CreateAsync(InteractionContext context, Func<InteractionContext, DiscordWebhookBuilder, Task<DiscordMessage>> createMessage);
    Task HandleItemInSlotAsync(Guid shopId, int slot, Func<DiscordWebhookBuilder, Task> updateMessage);
    Task SetActiveToolAsync(Guid shopId, ShopTool tool, Func<DiscordWebhookBuilder, Task> updateMessage);
    Task ReRollAsync(Guid shopId, Func<DiscordWebhookBuilder, Task> updateMessage);
}