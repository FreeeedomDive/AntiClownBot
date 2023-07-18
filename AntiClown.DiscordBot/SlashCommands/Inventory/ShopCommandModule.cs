using AntiClown.DiscordBot.Interactivity.Services.Shop;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Inventory;

public class ShopCommandModule : SlashCommandModuleWithMiddlewares
{
    public ShopCommandModule(
        ICommandExecutor commandExecutor,
        IShopService shopService
    ) : base(commandExecutor)
    {
        this.shopService = shopService;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Shop, "Открыть магазин")]
    public async Task GetShop(InteractionContext context)
    {
        await ExecuteAsync(context, async () => await shopService.CreateAsync(context, RespondToInteractionAsync));
    }

    private readonly IShopService shopService;
}