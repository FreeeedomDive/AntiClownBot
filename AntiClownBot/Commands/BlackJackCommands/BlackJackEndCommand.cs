using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.BlackJackCommands
{
    public class BlackJackEndCommand : BaseCommand
    {
        public BlackJackEndCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public async override void Execute(MessageCreateEventArgs e, SocialRatingUser user)
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
            return;
        }

        public override string Help()
        {
            return "End BlackJack :5Head:";
        }
    }
}
