using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Dev;

public class DailyResetCommandModule : SlashCommandModuleWithMiddlewares
{
    public DailyResetCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        Models.Lohotron.Lohotron lohotron
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.lohotron = lohotron;
    }

    [SlashCommand(Interactions.Commands.Dev_DailyReset, "Ручной ежедневный сброс (если автоматический не сработал)", false)]
    [SlashCommandPermissions(Permissions.ViewAuditLog)]
    public async Task ManualDailyReset(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            await apiClient.Users.DailyResetAsync();
            lohotron.Reset();
            await RespondToInteractionAsync(context, await discordClientWrapper.Emotes.FindEmoteAsync("YEP"));
        });
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly Models.Lohotron.Lohotron lohotron;
}