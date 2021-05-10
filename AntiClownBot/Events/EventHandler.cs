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

        private List<BaseEvent> _allEvents;

        public EventHandler(DiscordClient client)
        {
            BaseEvent.SetDiscordClient(client);
            NextEvents = new Queue<BaseEvent>();
            _allEvents = new List<BaseEvent>
            {
                new CloseTributesEvent(),
                new RemoveCooldownEvent.RemoveCooldownEvent(),
                new TransfusionEvent.TransfusionEvent(),
                new LotteryEvent.LotteryEvent()
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
            const double minHoursToSleep = 1;
            const double maxHoursToSleep = 4;
            while (true)
            {
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
                currentEvent.Execute();

                if (!currentEvent.HasRelatedEvents()) continue;

                NextEvents.Enqueue(currentEvent.RelatedEvents.SelectRandomItem());
            }
        }

        private static async void AddLog(string content)
        {
            using var file = new StreamWriter("log.txt", true);
            await file.WriteLineAsync($"{DateTime.Now} | {content}");
        }
    }
}