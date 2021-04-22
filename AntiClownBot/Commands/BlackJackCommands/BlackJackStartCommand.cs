using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackStartCommand : BaseCommand
    {
        public BlackJackStartCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public async override void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if(Config.CurrentBlackJack == null)
            {
                await e.Message.RespondAsync("BlackJack doesn't exist");
                return;
            }
            if(Config.CurrentBlackJack.IsActive)
            {
                await e.Message.RespondAsync("Already started");
                return;
            }
            await e.Message.RespondAsync(Config.CurrentBlackJack.StartRound());
            return;
        }

        public override string Help()
        {
            return "Start new round with joined players";
        }
    }
}
