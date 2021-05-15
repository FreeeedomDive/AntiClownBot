using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.SpecialChannels.BlackJack.Commands
{
    public class BlackJackStart : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public BlackJackStart(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "start";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack.IsActive)
            {
                return "Раунд уже начался";
            }

            if (Config.CurrentBlackJack.Players.All(player => player.Name != user.DiscordUsername))
            {
                return "Ты не принимаешь участие в игре";
            }

            return Config.CurrentBlackJack.StartRound();
        }
    }
}
