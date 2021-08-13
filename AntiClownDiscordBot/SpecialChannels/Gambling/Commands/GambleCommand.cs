using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Linq;

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

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble == null)
            {
                user.ChangeRating(-15);
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
                user.ChangeRating(-15);
                return $"Вместо ставки ты высрал какую-то хуйню, держи -15 {Utility.StringEmoji(":Pepega:")}";
            }

            if (bet <= 0)
            {
                user.ChangeRating(-50);
                return $"Ты серьезно думал меня наебать? Держи -50 {Utility.StringEmoji(":Pepega:")}";
            }
            var result = Config.CurrentGamble.MakeBid(user, option, bet);
            switch (result)
            {
                case GambleBetOptions.OptionDoesntExist:
                    return "Такого варианта не существует";
                case GambleBetOptions.NotEnoughRating:
                    return "Ты слишком лох, чтобы ставить такую сумму";
                case GambleBetOptions.SuccessfulBet:
                    return "Принята ставка";
                case GambleBetOptions.SuccessfulRaise:
                    return "Принято увеличение ставки";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
