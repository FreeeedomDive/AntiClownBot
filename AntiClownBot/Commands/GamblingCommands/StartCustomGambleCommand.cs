using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.GamblingCommands
{
    public class StartCustomGambleCommand : BaseCommand
    {
        public StartCustomGambleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble != null)
            {
                await e.Message.RespondAsync("В данный момент уже запущена ставка");
                return;
            }

            var message = e.Message.Content;
            var lines = message.Split('\n');
            var gambleName = string.Join(" ", lines[0].Split(' ').Skip(1));
            var optionsLines = lines.Skip(1).ToList();
            if (optionsLines.Count < 2)
            {
                await e.Message.RespondAsync("Вариантов исхода должно быть 2 и более");
                return;
            }

            var options = new List<GambleOption>();
            foreach (var line in optionsLines)
            {
                var args = line.Split(' ');
                var option = string.Join(" ", args.Take(args.Length - 1));
                var isParsed = float.TryParse(args.Last(), out var ratio);
                if (!isParsed)
                {
                    await e.Message.RespondAsync($"Значение {args.Last()} не является типом double");
                    return;
                }

                options.Add(new GambleOption(option, ratio));
            }

            Config.CurrentGamble = new Gamble(gambleName, e.Author.Id, GambleType.WithCustomRatio, options);
            Config.Save();
            await e.Message.RespondAsync($"Начата ставка \"{Config.CurrentGamble.GambleName}\"");
        }

        public override string Help()
        {
            return "Начало новой ставки с кастомными коэффициентами на каждый вариант\n" +
                   "Можно иметь только одну активную ставку\nИспользование:\n" +
                   "!startgamble [Название ставки]\n[Вариант 1 [Коэффициент1]]\n[Вариант 2 [Коэффициент2]]\n..." +
                   "\n[Вариант N [КоэффициентN]]";
        }
    }
}