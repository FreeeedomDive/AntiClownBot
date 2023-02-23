using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.EventServices;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.Lottery
{
    public class LotteryCommand : ICommand
    {
        public LotteryCommand(
            IDiscordClientWrapper discordClientWrapper,
            ILotteryService lotteryService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.lotteryService = lotteryService;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            if (lotteryService.Lottery is not { IsJoinable: true })
            {
                return;
            }

            if (lotteryService.Lottery.Participants.Contains(e.Author.Id))
            {
                await e.Message.RespondAsync("Ты уже участвуешь");
                return;
            }

            var message = await lotteryService.Lottery.Join(e.Author.Id);
            await discordClientWrapper.Messages.RespondAsync(e.Message, message);
        }

        public Task<string> Help()
        {
            var result = ">>> !!! Команда работает только во время эвента с лотереей !!!\n" +
                         "Принятие участия в активной лотерее\n" +
                         "Смайлики, которые могут выпасть:\n" +
                         string.Join("\n",
                             Models
                                 .Lottery
                                 .Lottery
                                 .GetAllEmotes()
                                 .OrderBy(Models.Lottery.Lottery.EmoteToInt)
                                 .Select(emote =>
                                     $"\t{discordClientWrapper.Emotes.FindEmoteAsync(emote.ToString()).GetAwaiter().GetResult()}" +
                                     $" = {Models.Lottery.Lottery.EmoteToInt(emote)}")) +
                         "\nКоличество одинаковых смайликов тоже влияет на результат\n" +
                         "1 смайлик = x1, 2 смайлика = x5, 3 смайлика = x10, 4 смайлика = x50, 5 смайликов = x100";

            return Task.FromResult(result);
        }

        public string Name => "lottery";
        public bool IsObsolete => false;

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly ILotteryService lotteryService;
    }
}