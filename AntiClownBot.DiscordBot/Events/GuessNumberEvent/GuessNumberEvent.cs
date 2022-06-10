using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.EventServices;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.GuessNumberEvent
{
    public class GuessNumberEvent : IEvent
    {
        public GuessNumberEvent(
            IDiscordClientWrapper discordClientWrapper,
            IGuildSettingsService guildSettingsService,
            IGuessNumberService guessNumberService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.guildSettingsService = guildSettingsService;
            this.guessNumberService = guessNumberService;
        }

        public async Task ExecuteAsync()
        {
            if (guessNumberService.CurrentGame != null)
                return;
            var message = await TellBackStory();
            guessNumberService.CreateGuessNumberGame(message.Id);
            await Task.Delay(10 * 60 * 1000);
            await guessNumberService.CurrentGame.MakeResult();
        }

        public async Task<DiscordMessage> TellBackStory()
        {
            var text = (DateTime.Now.IsNightTime() ? "" : "@everyone ")
                       + "Я загадал число, угадайте его!!\n"
                       + "У вас 10 минут";

            var message = await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, text);
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("one"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("two"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("three"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("four"));
            return message;
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;

        public List<IEvent> RelatedEvents => new();

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IGuessNumberService guessNumberService;
    }
}