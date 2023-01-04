using AntiClownDiscordBotVersion2.Events.NightEvents;
using AntiClownDiscordBotVersion2.Events.SpecialEventDays;
using AntiClownDiscordBotVersion2.Settings.EventSettings;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2.Events
{
    public class EventScheduler
    {
        public EventScheduler(
            IEventSettingsService eventSettingsService,
            IDailyStatisticsService dailyStatisticsService,
            ILoggerClient logger,
            IRandomizer randomizer,
            IEvent[] events,
            INightEvent[] nightEvents
        )
        {
            this.eventSettingsService = eventSettingsService;
            this.dailyStatisticsService = dailyStatisticsService;
            this.logger = logger;
            this.randomizer = randomizer;
            this.nightEvents = nightEvents;
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
            Task.Run(HandleNextEvent);
        }

        private async Task HandleNextEvent()
        {
            while (true)
            {
                var sleepTime = CalculateTimeBeforeNextEvent();
                var nextEventTime = DateTime.Now.AddMilliseconds(sleepTime);
                await logger.InfoAsync(
                    "Следующий эвент в {time}, через {diff}",
                    Utility.NormalizeTime(nextEventTime),
                    Utility.GetTimeDiff(nextEventTime));
                await Task.Delay(sleepTime);

                var eventDayTypeFromSettings = eventSettingsService.GetEventSettings().EventsType;
                var eventDayType = Enum.TryParse<EventDayType>(eventDayTypeFromSettings, out var t) ? t : EventDayType.CommonDay;
                var eventConfiguration = eventMappings[eventDayType];
                if (DateTime.Now.IsNightTime())
                {
                    await nightEvents.SelectRandomItem(randomizer).ExecuteAsync();
                    continue;
                }

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

        private static int CalculateTimeBeforeNextEvent()
        {
            // calculate time for scheduler start
            // events will start at the half of odd hours
            // example: 9:30, 11:30, 13:30 etc.
            const int eventStartHour = 1; //1 == odd, 0 = even
            const int eventStartMinute = 30; // xx:30 minutes
            var now = DateTime.Now;
            var secondsToSleep = 60 - now.Second;
            var minutesToSleep = now.Minute < eventStartMinute
                ? (eventStartMinute - now.Minute - 1)
                : (60 - now.Minute + eventStartMinute - 1);
            var hoursToSleep = now.Hour % 2 == eventStartHour
                ? (now.Minute < eventStartMinute ? 0 : 1)
                : (now.Minute < eventStartMinute ? 1 : 0);

            return (hoursToSleep * 60 * 60 + minutesToSleep * 60 + secondsToSleep) * 1000;
        }

        private readonly Queue<IEvent> nextEvents;

        private readonly IEventSettingsService eventSettingsService;
        private readonly IDailyStatisticsService dailyStatisticsService;
        private readonly ILoggerClient logger;
        private readonly IRandomizer randomizer;
        private readonly INightEvent[] nightEvents;
        private readonly Dictionary<EventDayType, ISpecialEventDay> eventMappings;
    }
}