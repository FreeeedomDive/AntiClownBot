using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.Lottery
{
    public class LotteryCommand : BaseCommand
    {
        public LotteryCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }
        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentLottery == null || !Config.CurrentLottery.IsJoinable)
                return;
            var message = Config.CurrentLottery.Join(user);
            await e.Message.RespondAsync(message);
        }

        public override string Help()
        {
            return "Работает только во время эвента с лотереей";
        }
    }
}
