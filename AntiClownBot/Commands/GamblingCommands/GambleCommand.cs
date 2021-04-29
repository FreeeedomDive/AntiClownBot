using System;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.GamblingCommands
{
    public class GambleCommand : BaseCommand
    {
        public GambleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble == null)
            {
                await e.Message.RespondAsync("В данный момент нет активной ставки, лудоман ебучий");
                user.DecreaseRating(15);
                return;
            }

            if (!Config.CurrentGamble.IsActive)
            {
                await e.Message.RespondAsync("На данную ставку больше нет сборов");
                return;
            }

            var message = e.Message.Content;
            var messageArgs = message.Split(' ');
            var option = string.Join(" ", messageArgs.Skip(1).Take(messageArgs.Length - 2));
            var betParsed = int.TryParse(messageArgs.Last(), out var bet);
            if (!betParsed)
            {
                await e.Message.RespondAsync($"Вместо ставки ты высрал какую-то хуйню, держи -15 {Utility.StringEmoji(":Pepega:")}");
                user.DecreaseRating(15);
                return;
            }

            if (bet <= 0)
            {
                await e.Message.RespondAsync($"Ты серьезно думал меня наебать? Держи -50 {Utility.StringEmoji(":Pepega:")}");
                user.DecreaseRating(50);
                return;
            }
            var result = Config.CurrentGamble.MakeBid(user, option, bet);
            switch (result)
            {
                case GambleBetOptions.OptionDoesntExist:
                    await e.Message.RespondAsync("Такого варианта не существует");
                    break;
                case GambleBetOptions.NotEnoughRating:
                    await e.Message.RespondAsync("Ты слишком лох, чтобы ставить такую сумму");
                    break;
                case GambleBetOptions.SuccessfulBet:
                    await e.Message.RespondAsync("Принята ставка");
                    break;
                case GambleBetOptions.SuccessfulRaise:
                    await e.Message.RespondAsync("Принято увеличение ставки");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Config.Save();
        }

        public override string Help()
        {
            return
                "Принятие участия в текущей активной ставке\n" +
                "Использование:\n!gamble [вариант исхода] [количество рейтинга, которое нужно поставить]\n" +
                "Повторное использование позволяет добавить рейтинг к ставке";
        }
    }
}