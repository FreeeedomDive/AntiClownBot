using System;
using DSharpPlus;

namespace AntiClownBot.Events.CloseTributeEvent.RelatedOpenTributesEvent
{
    public class OpenTributesEvent: BaseEvent
    {
        public override int EventCooldown => 1000;
        public override void ExecuteAsync()
        {
            Config.EventPossibleTimes["opentributes"] = DateTime.Now.AddMilliseconds(EventCooldown);
            TellBackStory();
            Config.OpenTributes();
        }

        protected override string BackStory()
        {
            return $"Так, я вернулся, несите свои подношения {Utility.StringEmoji(":BASED:")}";
        }
    }
}