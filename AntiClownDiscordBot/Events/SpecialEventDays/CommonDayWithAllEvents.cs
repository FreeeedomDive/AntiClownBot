using System.Collections.Generic;

namespace AntiClownBot.Events.SpecialEventDays
{
    public class CommonDayWithAllEvents: ISpecialEventDay
    {
        public int EventInterval => 2 * 60 * 60 * 1000;
        public List<BaseEvent> Events => new()
        {
            new RemoveCooldownEvent.RemoveCooldownEvent(),
            new TransfusionEvent.TransfusionEvent(),
            new LotteryEvent.LotteryEvent(),
            new GuessNumberEvent.GuessNumberEvent(),
            new MaliMaliEvent.MaliMaliEvent(),
            new RaceEvent.RaceEvent()
        };
    }
}