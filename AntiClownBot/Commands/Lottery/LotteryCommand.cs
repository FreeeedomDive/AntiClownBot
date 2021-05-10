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
            if (Config.CurrentLottery.Participants.Contains(user.DiscordId))
            {
                await e.Message.RespondAsync("Ты уже участвуешь");
                return;
            }

            var message = Config.CurrentLottery.Join(user);
            await e.Message.RespondAsync(message);
        }

        public override string Help()
        {
            return ">>> !!! Команда работает только во время эвента с лотереей !!!\n" +
                   "Принятие участия в активной лотерее\n" +
                   "Смайлики, которые могут выпасть:\n" +
                   string.Join("\n",
                       Models.Lottery.Lottery.GetAllEmotes().OrderBy(Models.Lottery.Lottery.EmoteToInt).Select(emote =>
                           $"\t{Utility.StringEmoji($":{emote.ToString()}:")} = {Models.Lottery.Lottery.EmoteToInt(emote)}")) +
                   "\nКоличество одинаковых смайликов тоже влияет на результат\n" +
                   "1 смайлик = x1, 2 смайлика = x5, 3 смайлика = x10, 4 смайлика = x50, 5 смайликов = x100";
        }
    }
}