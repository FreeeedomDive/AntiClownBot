using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Statistics.Emotes;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.StatsCommands
{
    public class EmojiStatsCommand : ICommand
    {
        public EmojiStatsCommand(
            IDiscordClientWrapper discordClientWrapper,
            IEmoteStatsService emoteStatsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.emoteStatsService = emoteStatsService;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, await emoteStatsService.GetStats());
        }

        public Task<string> Help()
        {
            return Task.FromResult("Статистика использованных на сервере эмоджи");
        }

        public string Name => "emotes";

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IEmoteStatsService emoteStatsService;
    }
}