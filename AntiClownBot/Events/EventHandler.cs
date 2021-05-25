using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AntiClownBot.Events.CloseTributeEvent;
using DSharpPlus;

namespace AntiClownBot.Events
{
    public class EventHandler
    {
        public Queue<BaseEvent> NextEvents;

        private readonly List<BaseEvent> _allEvents;

        public EventHandler(DiscordClient client)
        {
            BaseEvent.SetDiscordClient(client);
            NextEvents = new Queue<BaseEvent>();
            _allEvents = new List<BaseEvent>
            {
                new CloseTributesEvent(),
                new RemoveCooldownEvent.RemoveCooldownEvent(),
                new TransfusionEvent.TransfusionEvent(),
                new LotteryEvent.LotteryEvent(),
                new ShopEvent.ShopEvent(),
                new GuessNumberEvent.GuessNumberEvent()
            };
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
            var firstLaunch = false;
            while (true)
            {
                var minHoursToSleep = firstLaunch ? 0.05 : 1;
                var maxHoursToSleep = firstLaunch ? 0.05 : 3;
                firstLaunch = false;
                var sleepTime = Randomizer.GetRandomNumberBetween(
                    (int)(minHoursToSleep * 60 * 60 * 1000),
                    (int)(maxHoursToSleep * 60 * 60 * 1000)
                );
                var nextEventTime = DateTime.Now.AddMilliseconds(sleepTime);
                AddLog(
                    $"Следующий эвент в {Utility.NormalizeTime(nextEventTime)}, через {Utility.GetTimeDiff(nextEventTime)}");
                await Task.Delay(sleepTime);
                if (NextEvents.Count == 0)
                {
                    NextEvents.Enqueue(_allEvents.SelectRandomItem());
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