namespace AntiClownDiscordBotVersion2.Events.SpecialEventDays
{
    public class RaceDay : ISpecialEventDay
    {
        public RaceDay(IEvent raceEvent)
        {
            Events = new IEvent[]
            {
                raceEvent
            };
        }

        public IEvent[] Events { get; }
    }
}