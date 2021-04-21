using System;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.GamblingCommands
{
    public class GambleResultCommand: BaseCommand
    {
        public GambleResultCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble == null)
            {
                await e.Message.RespondAsync("В данный момент нет активной ставки");
                return;
            }

            var message = e.Message.Content;
            var options = message.Split('\n').Skip(1).ToArray();
            var result = Config.CurrentGamble.MakeGambleResult(e.Author.Id, options);
            switch (result)
            {
                case GambleBetResult.OptionDoesntExist:
                    await e.Message.RespondAsync("Одного из вариантов не существует, он не может быть верным");
                    break;
                case GambleBetResult.UserIsNotAuthor:
                    await e.Message.RespondAsync($"Ты кто такой, чел? Держи -30 {DiscordEmoji.FromName(DiscordClient, ":PogOff:")}");
                    user.DecreaseRating(30);
                    break;
                case GambleBetResult.SuccessfulResult:
                    await e.Message.RespondAsync(Config.CurrentGamble.Result);
                    Config.CurrentGamble = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Config.Save();
        }

        public override string Help()
        {
            return "Закрытие текущей ставки с выбором победившего исхода событий\nИспользование:\n!gambleresult\n[Исход1]\n...\n[Исход N]\n" +
                   "Закрыть ставку может только создавший ее пользователь";
        }
    }
}