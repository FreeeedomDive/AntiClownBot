using AntiClown.Api.Client;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Inventory;

public class ShopCommandModule : SlashCommandModuleWithMiddlewares
{
    public ShopCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Shop, "Открыть магазин")]
    public async Task GetShop(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            
        });
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IUsersCache usersCache;
}