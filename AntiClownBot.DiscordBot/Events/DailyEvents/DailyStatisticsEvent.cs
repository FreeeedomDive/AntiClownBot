using System.Text;
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
            var messageContentBuilder = new StringBuilder()
                .AppendLine($"Граждане наш Clown-City за день заработать {dailyStatisticsService.DailyStatistics.CreditsCollected}")
                .AppendLine($"Мною было проводить {dailyStatisticsService.DailyStatistics.EventsCount} событий");
            if (dailyStatisticsService.DailyStatistics.CreditsById.Count > 0)
            {
                var (majorKey, majorValue) = dailyStatisticsService.DailyStatistics.CreditsById.MaxBy(x => x.Value);
                var majorUserName = (await discordClientWrapper.Members.GetAsync(majorKey)).ServerOrUserName();
                var (bomjKey, bomjValue) = dailyStatisticsService.DailyStatistics.CreditsById.MinBy(x => x.Value);
                var bomjUserName = (await discordClientWrapper.Members.GetAsync(bomjKey)).ServerOrUserName();
                messageContentBuilder
                    .AppendLine($"Мажор дня - {majorUserName} : {majorValue}")
                    .Append($"Бомж дня - {bomjUserName} : {bomjValue}");
            }

            return await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageContentBuilder.ToString());
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;

        public List<IEvent> RelatedEvents => new();

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IDailyStatisticsService dailyStatisticsService;
    }
}