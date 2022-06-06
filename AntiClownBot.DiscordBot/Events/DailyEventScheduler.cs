namespace AntiClownDiscordBotVersion2.Events
{
    public class DailyEventScheduler
    {
        private static System.Timers.Timer timer;
        private readonly IDailyEvent[] dailyEvents;

        public DailyEventScheduler(IDailyEvent[] dailyEvents)
        {
            this.dailyEvents = dailyEvents;
        }

        public void Start()
        {
            Task.Run(HandleEvents);
        }

        private async Task HandleEvents()
        {
            while(true)
            {
                var nowTime = DateTime.Now;
                var scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 1, 0, 0);
                if (nowTime > scheduledTime)
                {
                    scheduledTime = scheduledTime.AddDays(1);
                }

                var sleepTime = (scheduledTime - DateTime.Now).TotalMilliseconds;
                await Task.Delay((int)sleepTime);

                foreach (var dailyEvent in dailyEvents)
                {
                    await dailyEvent.ExecuteAsync();
                }
            }
        }
    }
}