using System;
using System.Configuration;

namespace AntiClownBot.Events.TransfusionEvent
{
    public class TransfusionEvent : BaseEvent
    {
        public override int EventCooldown => 1000;

        public override void ExecuteAsync()
        {
            SocialRatingUser theRichestUser = null;
            var maxRating = -1;
            foreach (var user in Config.Users.Values)
            {
                if (user.NetWorth > maxRating)
                {
                    maxRating = user.NetWorth;
                    theRichestUser = user;
                }
            }

            var exchange = Randomizer.GetRandomNumberBetween(50, 100);
            var exchangeUser = theRichestUser;
            while (exchangeUser == theRichestUser)
            {
                exchangeUser = Config.Users.Values.SelectRandomItem();
            }
            
            SendMessageToChannel("Я решил выделить немного кредитов рандомному челу, " +
                                 "но свой бюджет я тратить не буду, возьму из кармана самого богатого " +
                                 $"{Utility.StringEmoji(":MEGALUL:")} {Utility.StringEmoji(":point_right:")} {theRichestUser?.DiscordUsername}. " +
                                 $"Отдай {exchangeUser.DiscordUsername} {exchange} social credits");

            theRichestUser?.ChangeRating(-exchange);
            exchangeUser.ChangeRating(exchange);
        }

        protected override string BackStory()
        {
            return "";
        }
    }
}