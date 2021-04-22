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
            if (Config.CurrentBlackJack.Players.Peek().Name != user.DiscordUsername)
            {
                await e.Message.RespondAsync("Not your turn");
                return;
            }

            var player = Config.CurrentBlackJack.Players.Dequeue();
            Config.CurrentBlackJack.Players.Enqueue(player);
            if(Config.CurrentBlackJack.Players.Peek().IsDealer)
            {
                await e.Message.RespondAsync(Config.CurrentBlackJack.MakeResult());
                Config.Save();
                return;
            }
            
            await e.Message.RespondAsync($"{Config.CurrentBlackJack.Players.Peek().Name} , your turn");
            Config.Save();
        }

        public override string Help()
        {
            return "Take no more cards";
        }
    }
}
