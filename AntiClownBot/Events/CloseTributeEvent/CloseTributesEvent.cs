﻿using System.Collections.Generic;
using AntiClownBot.Events.CloseTributeEvent.RelatedOpenTributesEvent;
using DSharpPlus;

namespace AntiClownBot.Events.CloseTributeEvent
{
    public class CloseTributesEvent : BaseEvent
    {
        public CloseTributesEvent()
        {
            RelatedEvents = new List<BaseEvent>
            {
                new OpenTributesEvent()
            };
        }

        public override void Execute()
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