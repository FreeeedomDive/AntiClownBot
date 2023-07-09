using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.RemoveCooldownEvent
{
    public class RemoveCooldownEvent : IEvent
    {
        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IEmotesProvider emotesProvider;
        private readonly IApiClient apiClient;
        private readonly IGuildSettingsService guildSettingsService;

        public RemoveCooldownEvent(
            IDiscordClientWrapper discordClientWrapper,
            IEmotesProvider emotesProvider,
            IApiClient apiClient,
            IGuildSettingsService guildSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.emotesProvider = emotesProvider;
            this.apiClient = apiClient;
            this.guildSettingsService = guildSettingsService;
        }
        
        public async Task ExecuteAsync()
        {
            await TellBackStory();
            await apiClient.Users.RemoveCooldownsAsync();
        }

        public async Task<DiscordMessage> TellBackStory()
        {
            var messageContent =
                $"У меня хорошее настроение, несите ваши подношения {await emotesProvider.GetEmoteAsTextAsync("peepoClap")}";
            return await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageContent);
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;
        public List<IEvent> RelatedEvents => new();
    }
}