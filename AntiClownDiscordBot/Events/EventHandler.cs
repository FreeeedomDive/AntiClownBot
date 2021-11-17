using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AntiClownBot.Events.SpecialEventDays;
using AntiClownBot.Helpers;
using DSharpPlus;

namespace AntiClownBot.Events
{
    public class EventHandler
    {
        public Queue<BaseEvent> NextEvents;
        private EventDayType _eventDayType;
        private Dictionary<EventDayType, ISpecialEventDay> _eventMappings;

        public EventHandler(DiscordClient client)
        {
            BaseEvent.SetDiscordClient(client);
            _eventMappings = new Dictionary<EventDayType, ISpecialEventDay>()
            {
                {EventDayType.CommonDay, new CommonDayWithAllEvents()},
                {EventDayType.LotteryDay, new LotteryDay()},
                {EventDayType.RaceDay, new RaceDay()}
            };
            _eventDayType = EventDayType.CommonDay;
            NextEvents = new Queue<BaseEvent>();
        }

        public void Start()
        {
            var thread = new Thread(HandleNextEvent)
            {
                IsBackground = true
            };
            thread.Start();
        }

        private async void HandleNextEvent()
        {
            while (true)
            {
                var eventConfiguration = _eventMappings[_eventDayType];
                var sleepTime = eventConfiguration.EventInterval;
                var nextEventTime = DateTime.Now.AddMilliseconds(sleepTime);
                AddLog($"Следующий эвент в {Utility.NormalizeTime(nextEventTime)}, " +
                       $"через {Utility.GetTimeDiff(nextEventTime)}");
                await Task.Delay(sleepTime);
                if (NextEvents.Count == 0)
                {
                    NextEvents.Enqueue(eventConfiguration.Events.SelectRandomItem());
                }

                var currentEvent = NextEvents.Dequeue();
                Configuration.GetConfiguration().DailyStatistics.EventsCount++;
                currentEvent.ExecuteAsync();

                if (!currentEvent.HasRelatedEvents()) continue;

                NextEvents.Enqueue(currentEvent.RelatedEvents.SelectRandomItem());
            }
        }

        private static void AddLog(string content)
        {
            NLogWrapper.GetDefaultLogger().Info(content);
        }
    }
}