namespace AntiClownDiscordBotVersion2.Events.SpecialEventDays
{
    public class CommonDayWithAllEvents: ISpecialEventDay
    {
        public CommonDayWithAllEvents(IEvent[] events)
        {
            Events = events;
        }

        public IEvent[] Events { get; }
    }
}