using AntiClown.Api.Client;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

public class DailyResetCommandModule : SlashCommandModuleWithMiddlewares
{
    public DailyResetCommandModule(
        ICommandExecutor commandExecutor,
        IEmotesCache emotesCache,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient
    ) : base(commandExecutor)
    {
        this.emotesCache = emotesCache;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_DailyReset, "Ручной ежедневный сброс (если автоматический не сработал)", false)]
    [SlashCommandPermissions(Permissions.ViewAuditLog)]
    public async Task ManualDailyReset(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            await antiClownEntertainmentApiClient.DailyEvents.PaymentsAndResets.StartNewAsync();
            await RespondToInteractionAsync(context, await emotesCache.GetEmoteAsTextAsync("YEP"));
        });
    }

    private readonly IEmotesCache emotesCache;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}