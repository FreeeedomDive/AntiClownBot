namespace AntiClownDiscordBotVersion2.Events.SpecialEventDays
{
    public class LotteryDay: ISpecialEventDay
    {
        public LotteryDay(IEvent lotteryEvent)
        {
            Events = new[]
            {
                lotteryEvent
            };
        }
        
        public IEvent[] Events { get; }
    }
}