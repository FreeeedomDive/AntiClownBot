using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.DailyEvents
{
    public class DailyStatisticsEvent : IDailyEvent
    {
        public DailyStatisticsEvent(
            IDiscordClientWrapper discordClientWrapper,
            IGuildSettingsService guildSettingsService,
            IDailyStatisticsService dailyStatisticsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.guildSettingsService = guildSettingsService;
            this.dailyStatisticsService = dailyStatisticsService;
        }

        public async Task ExecuteAsync()
        {
            await TellBackStory();
            dailyStatisticsService.ClearDayStatistics();
            dailyStatisticsService.Save();
        }

        public async Task<DiscordMessage> TellBackStory()
        {
            var (majorKey, majorValue) = dailyStatisticsService.DailyStatistics.CreditsById.OrderBy(x => x.Value).Last();
            var majorUserName = (await discordClientWrapper.Members.GetAsync(majorKey)).ServerOrUserName();
            var (bomjKey, bomjValue) = dailyStatisticsService.DailyStatistics.CreditsById.OrderBy(x => x.Value).First();
            var bomjUserName = (await discordClientWrapper.Members.GetAsync(bomjKey)).ServerOrUserName();

            var messageContent = "Добрый вечер, Clown-City!\n" +
                   $"Граждане наш Clown-City за день заработать {dailyStatisticsService.DailyStatistics.CreditsCollected}\n" +
                   $"Мною было проводить {dailyStatisticsService.DailyStatistics.EventsCount} событий\n" +
                   $"Мажор дня - {majorUserName} : {majorValue}\n" +
                   $"Бомж дня - {bomjUserName} : {bomjValue}";
            return await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageContent);
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;

        public List<IEvent> RelatedEvents => new();

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IDailyStatisticsService dailyStatisticsService;
    }
}