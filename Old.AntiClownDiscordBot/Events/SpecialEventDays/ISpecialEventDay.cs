using System.Collections.Generic;

namespace AntiClownBot.Events.SpecialEventDays
{
    public interface ISpecialEventDay
    {
        public int EventInterval { get; }
        public List<BaseEvent> Events { get; }
    }
}