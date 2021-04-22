using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackStandCommand : BaseCommand
    {
        public BlackJackStandCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("BlackJack doesn't exist");
                return;
            }
            if (Config.CurrentBlackJack.Players.All(player => player.Name != user.DiscordUsername))
            {
                await e.Message.RespondAsync("You are currently not participating in BlackJack");
                return;
            }
            if (!Config.CurrentBlackJack.IsActive)
            {
                await e.Message.RespondAsync("Round has not started yet");
                return;
            }
            if (Config.CurrentBlackJack.CurrentPlayer.Name != user.DiscordUsername)
            {
                await e.Message.RespondAsync("Not your turn");
                return;
            }
            
            Config.CurrentBlackJack.CurrentPlayer = Config.CurrentBlackJack.CurrentPlayer.NextPlayer;
            if(Config.CurrentBlackJack.CurrentPlayer.NextPlayer == null)
            {
                await e.Message.RespondAsync(Config.CurrentBlackJack.MakeResult());
                Config.Save();
                return;
            }
            
            await e.Message.RespondAsync($"{Config.CurrentBlackJack.CurrentPlayer.Name} , your turn");
            Config.Save();
        }

        public override string Help()
        {
            return "Take no more cards";
        }
    }
}
