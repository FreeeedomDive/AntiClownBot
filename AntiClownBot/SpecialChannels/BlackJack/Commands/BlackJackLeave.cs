using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.BlackJack.Commands
{
    public class BlackJackLeave : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public BlackJackLeave(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "leave";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack.Players.All(player => player.Name != user.DiscordUsername))
            {
                return "Ты не принимаешь участие в игре";
            }
            if(Config.CurrentBlackJack.Players.First().UserId == user.DiscordId)
            {
                Config.CurrentBlackJack.StopTimer();
            }
            var message = (Config.CurrentBlackJack.Leave(user));
            if(Config.CurrentBlackJack.Players.First().IsDealer)
            {
                Config.CurrentBlackJack.StopTimer();
                message += Config.CurrentBlackJack.MakeResult();
            }
            return message;
        }
    }
}
