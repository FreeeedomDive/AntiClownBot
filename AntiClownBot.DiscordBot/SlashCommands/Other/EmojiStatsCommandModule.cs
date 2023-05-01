using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.Statistics.Emotes;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class EmojiStatsCommandModule : SlashCommandModuleWithMiddlewares
{
    public EmojiStatsCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IEmoteStatsService emoteStatsService
    ) : base(commandExecutor)
    {
        this.emoteStatsService = emoteStatsService;
    }

    [SlashCommand("emoji", "Статистика использованных на сервере эмоджи")]
    public async Task GetStats(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            await RespondToInteractionAsync(context, await emoteStatsService.GetStats());
        });
    }

    private readonly IEmoteStatsService emoteStatsService;
}