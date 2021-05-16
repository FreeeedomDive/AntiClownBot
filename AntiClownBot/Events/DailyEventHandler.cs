using AntiClownBot.Events.DailyEvents;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiClownBot.Events
{
    public class DailyEventHandler
    {
        private static System.Timers.Timer timer;
        private readonly List<BaseEvent> _dailyEvents;

        public DailyEventHandler()
        {
            _dailyEvents = new List<BaseEvent>
            {
                new PayoutsDailyEvent(),
                new DailyStatisticsEvent()
            };
        }

        public void Start()
        {
            var thread = new Thread(StartTimer)
            {
                IsBackground = true
            };
            thread.Start();
        }

        public void StartTimer()
        {
            var nowTime = DateTime.Now;
            var scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 1, 0, 0);
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            var tickTime = (scheduledTime - DateTime.Now).TotalMilliseconds;
            timer = new System.Timers.Timer(tickTime);
            timer.Elapsed += HandleEvents;
            timer.Start();
        }

        private void HandleEvents(object sender, System.Timers.ElapsedEventArgs e)
        {
            AddLog($"!!!Timer stopped!!!");
            timer.Stop();
            foreach (var dailyEvent in _dailyEvents)
            {
                dailyEvent.ExecuteAsync();
            }

            StartTimer();
        }

        private static async void AddLog(string content)
        {
            using var file = new StreamWriter("log.txt", true);
            await file.WriteLineAsync($"{DateTime.Now} | {content}");
        }
    }
}