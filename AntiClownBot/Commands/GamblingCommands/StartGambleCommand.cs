using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.GamblingCommands
{
    public class StartGambleCommand : BaseCommand
    {
        public StartGambleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
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

            var options = optionsLines.Select(option => new GambleOption(option, 0f)).ToList();

            Config.CurrentGamble = new Gamble(gambleName, e.Author.Id, GambleType.Default, options);
            Config.Save();
            await e.Message.RespondAsync($"Начата ставка \"{Config.CurrentGamble.GambleName}\"");
        }

        public override string Help()
        {
            return "Начало новой ставки\nМожно иметь только одну активную ставку\nИспользование:\n" +
                   "!startgamble [Название ставки]\n[Вариант 1]\n[Вариант 2]\n...\n[Вариант N]";
        }
    }
}