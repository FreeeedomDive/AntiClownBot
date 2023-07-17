using AntiClown.DiscordBot.Interactivity.Domain.Inventory;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.Interactivity.Services.Inventory;

public interface IInventoryService
{
    Task CreateAsync(InteractionContext context, Func<InteractionContext, DiscordWebhookBuilder, Task<DiscordMessage>> createMessage);
    Task HandleItemInSlotAsync(Guid inventoryId, int slot, Func<DiscordWebhookBuilder, Task> updateMessage);
    Task SetActiveToolAsync(Guid inventoryId, InventoryTool tool, Func<DiscordWebhookBuilder, Task> updateMessage);
    Task ChangePageAsync(Guid inventoryId, int pageDiff, Func<DiscordWebhookBuilder, Task> updateMessage);
}