using AntiClownDiscordBotVersion2.Utils;
using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2.Events
{
    public class DailyEventScheduler
    {
        public DailyEventScheduler(
            IDailyEvent[] dailyEvents,
            ILoggerClient logger
        )
        {
            this.dailyEvents = dailyEvents;
            this.logger = logger;
        }

        public void Start()
        {
            Task.Run(HandleEvents);
        }

        private async Task HandleEvents()
        {
            while (true)
            {
                var nowTime = DateTime.Now;
                var scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 1, 0, 0);
                if (nowTime > scheduledTime)
                {
                    scheduledTime = scheduledTime.AddDays(1);
                }

                await logger.InfoAsync($"Следующий ежедневный эвент в {Utility.NormalizeTime(scheduledTime)}, " +
                                  $"через {Utility.GetTimeDiff(scheduledTime)}");

                var sleepTime = (scheduledTime - DateTime.Now).TotalMilliseconds;
                await Task.Delay((int)sleepTime);

                foreach (var dailyEvent in dailyEvents)
                {
                    await logger.InfoAsync("Executing {Event}", dailyEvent.GetType().Name);
                    await dailyEvent.ExecuteAsync();
                }
            }
        }

        private readonly IDailyEvent[] dailyEvents;
        private readonly ILoggerClient logger;
    }
}