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

        public string Execute(MessageCreateEventArgs e)
        {
            if (Config.CurrentBlackJack.IsActive)
            {
                return "Раунд уже начался";
            }

            if (Config.CurrentBlackJack.Players.All(player => player.UserId != e.Author.Id))
            {
                return "Ты не принимаешь участие в игре";
            }
            
            var message =  Config.CurrentBlackJack.StartRound();
            if(Config.CurrentBlackJack.IsActive)
                Config.CurrentBlackJack.StartTimer();
            return message;
        }
    }
}
