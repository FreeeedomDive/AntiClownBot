using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackDoubleCommand : BaseCommand
    {
        public BlackJackDoubleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
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
            if(Config.CurrentBlackJack.Players.Peek().DidHit)
            {
                await e.Message.RespondAsync("You can not double");
                return;
            }
            
            var result = Config.CurrentBlackJack.GetCard(true, Config.CurrentBlackJack.Players.Peek());
            switch (result.Status)
            {
                case Models.BlackJack.GetResultStatus.OK:
                    await e.Message.RespondAsync(result.Message);
                    break;
                case Models.BlackJack.GetResultStatus.NextPlayer:
                    var player = Config.CurrentBlackJack.Players.Dequeue();
                    Config.CurrentBlackJack.Players.Enqueue(player);
                    if (Config.CurrentBlackJack.Players.Peek().IsDealer)
                    {
                        await e.Message.RespondAsync(result.Message + Config.CurrentBlackJack.MakeResult());
                    }
                    else
                    {
                        await e.Message.RespondAsync(result.Message + $"{Config.CurrentBlackJack.Players.Peek().Name} , your turn");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Config.Save();
        }

        public override string Help()
        {
            return " The player is allowed to increase the initial bet up to 100% in exchange for committing to stand after receiving exactly one more card.";
        }
    }
}
