using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.DailyEvents
{
    public class PayoutsDailyEvent : IDailyEvent
    {
        public PayoutsDailyEvent(
            IDiscordClientWrapper discordClientWrapper,
            IGuildSettingsService guildSettingsService,
            IDailyStatisticsService dailyStatisticsService,
            IApiClient apiClient
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.guildSettingsService = guildSettingsService;
            this.dailyStatisticsService = dailyStatisticsService;
            this.apiClient = apiClient;
        }
        
        public async Task ExecuteAsync()
        {
            await TellBackStory();
            var allUsers = await apiClient.Users.GetAllUsersAsync();

            await apiClient.Users.BulkChangeUserBalanceAsync(allUsers.Users, 150, "Ежедневные выплаты");
            await apiClient.Users.DailyResetAsync();
        }
        
        public async Task<DiscordMessage> TellBackStory()
        {
            var messageContent = "Ежедневные выплаты!!! Сброс цены реролла в магазине и бесплатных распознавателей!!!";
            return await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageContent);
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;

        public List<IEvent> RelatedEvents => new();

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IDailyStatisticsService dailyStatisticsService;
        private readonly IApiClient apiClient;
    }
}
