using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class DailyResetCommandModule : ApplicationCommandModule
{
    public DailyResetCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        Models.Lohotron.Lohotron lohotron
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.lohotron = lohotron;
    }

    [SlashCommand("dailyReset", "Ручной ежедневный сброс (если автоматический не сработал)", false)]
    [SlashCommandPermissions(Permissions.Administrator)]
    public async Task ManualDailyReset(InteractionContext context)
    {
        await apiClient.Users.DailyResetAsync();
        lohotron.Reset();
        await discordClientWrapper.Messages.RespondAsync(context, await discordClientWrapper.Emotes.FindEmoteAsync("YEP"));
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly Models.Lohotron.Lohotron lohotron;
}