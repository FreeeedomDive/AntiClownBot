using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackEndCommand : BaseCommand
    {
        public BlackJackEndCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("BlackJack doesn't exist");
                return;
            }
            
            if(Config.CurrentBlackJack.IsActive)
            {
                await e.Message.RespondAsync("Cannot end BlackJack, cuz current round is not ended");
                return;
            }
            
            Config.CurrentBlackJack = null;
            await e.Message.RespondAsync("BlackJack Ended");
            Config.Save();
        }

        public override string Help()
        {
            return $"End BlackJack {DiscordEmoji.FromName(DiscordClient, ":5Head:")}";
        }
    }
}
