using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

public class RefreshUsersCacheCommandModule : SlashCommandModuleWithMiddlewares
{
    public RefreshUsersCacheCommandModule(
        ICommandExecutor commandExecutor,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_RefreshUsersCache, "Обновить кеш юзеров", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
    public async Task RefreshUsersCache(InteractionContext interactionContext)
    {
        await ExecuteAsync(
            interactionContext,
            async () =>
            {
                await usersCache.InitializeAsync();
                await RespondToInteractionAsync(interactionContext, "Кэш пользователей обновлен");
            }
        );
    }

    private readonly IUsersCache usersCache;
}