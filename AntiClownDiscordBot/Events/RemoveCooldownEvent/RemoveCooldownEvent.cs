using System;
using AntiClownBot.Helpers;

namespace AntiClownBot.Events.RemoveCooldownEvent
{
    public class RemoveCooldownEvent: BaseEvent
    {
        public override async void ExecuteAsync()
        {
            await TellBackStory();
            
            ApiWrapper.Wrappers.UsersApi.RemoveCooldowns();
        }

        protected override string BackStory()
        {
            return $"У меня хорошее настроение, несите ваши подношения {Utility.StringEmoji(":peepoClap:")}";
        }
    }
}