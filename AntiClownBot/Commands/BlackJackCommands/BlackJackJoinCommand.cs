using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackJoinCommand : BaseCommand
    {
        public BlackJackJoinCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if(Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("BlackJack doesn't exist");
                return;
            }
            
            if(Config.CurrentBlackJack.Players.Any(player => player.Name == user.DiscordUsername))
            {
                await e.Message.RespondAsync("Already joined");
                return;
            }
            
            if (Config.CurrentBlackJack.IsActive)
            {
                await e.Message.RespondAsync("Round has already started");
                return;
            }
            
            await e.Message.RespondAsync(Config.CurrentBlackJack.Join(user));
            Config.Save();
        }

        public override string Help()
        {
            return "Join BlackJack, ur bid always 50";
        }
    }
}
