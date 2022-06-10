using System.Collections.Generic;

namespace AntiClownBot.Events.SpecialEventDays
{
    public class RaceDay : ISpecialEventDay
    {
        public int EventInterval => (int) (0.5 * 60 * 60 * 1000);

        public List<BaseEvent> Events => new()
        {
            new RaceEvent.RaceEvent()
        };
    }
}