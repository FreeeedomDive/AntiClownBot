using AntiClown.DiscordBot.Interactivity.Services.Inventory;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Inventory;

public class InventoryCommandModule : SlashCommandModuleWithMiddlewares
{
    public InventoryCommandModule(
        ICommandExecutor commandExecutor,
        IInventoryService inventoryService
    ) : base(commandExecutor)
    {
        this.inventoryService = inventoryService;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Inventory, "Посмотреть свой инветарь")]
    public async Task GetInventory(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                await inventoryService.CreateAsync(context, RespondToInteractionAsync);
            }
        );
    }

    private readonly IInventoryService inventoryService;
}