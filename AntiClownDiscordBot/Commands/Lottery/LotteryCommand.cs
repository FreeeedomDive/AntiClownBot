using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Linq;

namespace AntiClownBot.Commands.Lottery
{
    public class LotteryCommand : BaseCommand
    {
        public LotteryCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            if (Config.CurrentLottery is not {IsJoinable: true})
                return;
            if (Config.CurrentLottery.Participants.Contains(e.Author.Id))
            {
                await e.Message.RespondAsync("Ты уже участвуешь");
                return;
            }

            var message = Config.CurrentLottery.Join(e.Author.Id);
            await e.Message.RespondAsync(message);
        }

        public override string Help()
        {
            return ">>> !!! Команда работает только во время эвента с лотереей !!!\n" +
                   "Принятие участия в активной лотерее\n" +
                   "Смайлики, которые могут выпасть:\n" +
                   string.Join("\n",
                       Models
                           .Lottery
                           .Lottery
                           .GetAllEmotes()
                           .OrderBy(Models.Lottery.Lottery.EmoteToInt)
                           .Select(emote =>
                               $"\t{Utility.StringEmoji($":{emote.ToString()}:")} = {Models.Lottery.Lottery.EmoteToInt(emote)}")) +
                   "\nКоличество одинаковых смайликов тоже влияет на результат\n" +
                   "1 смайлик = x1, 2 смайлика = x5, 3 смайлика = x10, 4 смайлика = x50, 5 смайликов = x100";
        }
    }
}