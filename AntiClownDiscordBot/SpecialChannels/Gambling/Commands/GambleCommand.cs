using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Linq;
using AntiClownBot.Models.Gamble;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleCommand : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleCommand(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "gamble";

        public string Execute(MessageCreateEventArgs e)
        {
            if (Config.CurrentGamble == null)
            {
                Config.ChangeBalance(e.Author.Id, -15, "Ставки нет, а чел лудоманыч...");
                return "В данный момент нет активной ставки, лудоман ебучий";
            }

            if (!Config.CurrentGamble.IsActive)
            {
                return "На данную ставку больше нет сборов";
            }

            var message = e.Message.Content;
            var messageArgs = message.Split(' ');
            var option = string.Join(" ", messageArgs.Skip(1).Take(messageArgs.Length - 2));
            var betParsed = int.TryParse(messageArgs.Last(), out var bet);
            if (!betParsed)
            {
                Config.ChangeBalance(e.Author.Id, -15, "Чел написал что-то левое вместо размера ставки");
                return $"Вместо ставки ты высрал какую-то хуйню, держи -15 {Utility.StringEmoji(":Pepega:")}";
            }

            if (bet <= 0)
            {
                Config.ChangeBalance(e.Author.Id, -50, "Чел попробовал ввести отрицательную ставку");
                return $"Ты серьезно думал меня наебать? Держи -50 {Utility.StringEmoji(":Pepega:")}";
            }
            var result = Config.CurrentGamble.MakeBid(e.Author.Id, option, bet);
            return result switch
            {
                GambleBetOptions.OptionDoesntExist => "Такого варианта не существует",
                GambleBetOptions.NotEnoughRating => "Ты слишком лох, чтобы ставить такую сумму",
                GambleBetOptions.SuccessfulBet => "Принята ставка",
                GambleBetOptions.SuccessfulRaise => "Принято увеличение ставки",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
