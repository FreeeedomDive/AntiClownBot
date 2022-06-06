namespace AntiClownDiscordBotVersion2.Models.DailyStatistics
{
    public class DailyStatistics
    {
        public int CreditsCollected { get; set; }
        public int EventsCount { get; set; }
        public Dictionary<ulong, int> CreditsById { get; set; }

        public DailyStatistics()
        {
            CreditsCollected = 0;
            EventsCount = 0;
            CreditsById = new Dictionary<ulong, int>();
        }
    }
}