using System.Collections.Generic;
using AntiClownBot.Events.CloseTributeEvent.RelatedOpenTributesEvent;
using DSharpPlus;

namespace AntiClownBot.Events.CloseTributeEvent
{
    public class CloseTributesEvent : BaseEvent
    {
        public override int EventCooldown => 1000;
        public CloseTributesEvent()
        {
            RelatedEvents = new List<BaseEvent>
            {
                new OpenTributesEvent()
            };
        }

        public override void ExecuteAsync()
        {
            TellBackStory();
            Config.CloseTributes();
            Config.Save();
        }

        protected override string BackStory()
        {
            return
                $"Как же вы заебали со своими бездарными подношениями, я беру перерыв {Utility.StringEmoji(":PogOff:")}";
        }
    }
}