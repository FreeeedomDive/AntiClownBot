using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Statistics.Emotes;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class EmojiStatsCommandModule : ApplicationCommandModule
{
    public EmojiStatsCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IEmoteStatsService emoteStatsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emoteStatsService = emoteStatsService;
    }

    [SlashCommand("emoji", "Статистика использованных на сервере эмоджи")]
    public async Task GetStats(InteractionContext context)
    {
        await discordClientWrapper.Messages.RespondAsync(context, await emoteStatsService.GetStats());
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmoteStatsService emoteStatsService;
}