using AntiClownDiscordBotVersion2.Events.SpecialEventDays;
using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Settings.EventSettings;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;

namespace AntiClownDiscordBotVersion2.Events
{
    public class EventScheduler
    {
        public EventScheduler(
            IEventSettingsService eventSettingsService,
            IDailyStatisticsService dailyStatisticsService,
            ILogger logger,
            IRandomizer randomizer,
            IEvent[] events
        )
        {
            this.eventSettingsService = eventSettingsService;
            this.dailyStatisticsService = dailyStatisticsService;
            this.logger = logger;
            this.randomizer = randomizer;
            eventMappings = new Dictionary<EventDayType, ISpecialEventDay>()
            {
                { EventDayType.CommonDay, new CommonDayWithAllEvents(events) },
                { EventDayType.LotteryDay, new LotteryDay(events.First(e => e is LotteryEvent.LotteryEvent)) },
                { EventDayType.RaceDay, new RaceDay((events.First(e => e is RaceEvent.RaceEvent))) }
            };
            nextEvents = new Queue<IEvent>();
        }

        public void Start()
        {
            logger.Info("ЗАПУСТИЛ ЕБАНЫЙ ПОТОК ШЕДУЛЕРА ЭВЕНТОВ");
            Task.Run(HandleNextEvent);
        }

        private async Task HandleNextEvent()
        {
            logger.Info("ЗАШЕЛ В ЕБАНЫЙ МЕТОД С ВАЙЛ ТРУ");
            while (true)
            {
                logger.Info($"{Task.CurrentId}");
                logger.Info("ЗАШЕЛ В ЕБАНЫЙ ВАЙЛ ТРУ В МЕТОДЕ ЭВЕНТОВ");
                var eventDayTypeFromSettings = eventSettingsService.GetEventSettings().EventsType;
                var eventDayType = Enum.TryParse<EventDayType>(eventDayTypeFromSettings, out var t) ? t : EventDayType.CommonDay;
                var eventConfiguration = eventMappings[eventDayType];
                var sleepTime = eventSettingsService.GetEventSettings().EventIntervalInMinutes;
                var nextEventTime = DateTime.Now.AddMilliseconds(sleepTime * 60 * 1000);
                AddLog($"Следующий эвент в {Utility.NormalizeTime(nextEventTime)}, " +
                       $"через {Utility.GetTimeDiff(nextEventTime)}");
                await Task.Delay(sleepTime);
                if (nextEvents.Count == 0)
                {
                    nextEvents.Enqueue(eventConfiguration.Events.SelectRandomItem(randomizer));
                }

                var currentEvent = nextEvents.Dequeue();
                dailyStatisticsService.DailyStatistics.EventsCount++;
                await currentEvent.ExecuteAsync();

                if (!currentEvent.HasRelatedEvents()) continue;

                nextEvents.Enqueue(currentEvent.RelatedEvents.SelectRandomItem(randomizer));
            }
        }

        private void AddLog(string content)
        {
            logger.Info(content);
        }

        private readonly Queue<IEvent> nextEvents;

        private readonly IEventSettingsService eventSettingsService;
        private readonly IDailyStatisticsService dailyStatisticsService;
        private readonly ILogger logger;
        private readonly IRandomizer randomizer;
        private readonly Dictionary<EventDayType, ISpecialEventDay> eventMappings;
    }
}