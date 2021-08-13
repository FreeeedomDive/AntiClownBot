using AntiClownBot.Models.Gamble;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleStartCustom : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleStartCustom(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "startcustom";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (Config.CurrentGamble != null)
            {
                return "В данный момент уже запущена ставка";
            }

            var message = e.Message.Content;
            var lines = message.Split('\n');
            var gambleName = string.Join(" ", lines[0].Split(' ').Skip(1));
            var optionsLines = lines.Skip(1).ToList();
            if (optionsLines.Count < 2)
            {
                return "Вариантов исхода должно быть 2 и более";
            }

            var options = new List<GambleOption>();
            foreach (var line in optionsLines)
            {
                var args = line.Split(' ');
                var option = string.Join(" ", args.Take(args.Length - 1));
                var isParsed = float.TryParse(args.Last(), out var ratio);
                if (!isParsed)
                {
                    return $"Значение {args.Last()} не является типом double";
                }

                options.Add(new GambleOption(option, ratio));
            }

            Config.CurrentGamble = new Gamble(gambleName, e.Author.Id, GambleType.WithCustomRatio, options);
            return $"Начата ставка \"{Config.CurrentGamble.GambleName}\"";
        }
    }
}
