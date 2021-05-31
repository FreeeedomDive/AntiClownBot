using System;

namespace AntiClownBot.Events.RemoveCooldownEvent
{
    public class RemoveCooldownEvent: BaseEvent
    {
        public override int EventCooldown => 1000;
        public override void ExecuteAsync()
        {
            Config.EventPossibleTimes["removecooldown"] = DateTime.Now.AddMilliseconds(EventCooldown);
            TellBackStory();
            
            foreach (var user in Config.Users.Values)
            {
                user.NextTribute = DateTime.Now;
            }

            Config.Save();
        }

        protected override string BackStory()
        {
            return $"У меня хорошее настроение, несите ваши подношения {Utility.StringEmoji(":peepoClap:")}";
        }
    }
}