using System;

namespace AntiClownBot.Events.RemoveCooldownEvent
{
    public class RemoveCooldownEvent: BaseEvent
    {
        public override void ExecuteAsync()
        {
            TellBackStory();
            
            ApiWrapper.Wrappers.UsersApi.RemoveCooldowns();
        }

        protected override string BackStory()
        {
            return $"У меня хорошее настроение, несите ваши подношения {Utility.StringEmoji(":peepoClap:")}";
        }
    }
}