using AntiClownBot.Models.Gamble;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Linq;

namespace AntiClownBot.SpecialChannels.Gambling.Commands
{
    public class GambleStart : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public GambleStart(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "start";

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

            var options = optionsLines.Select(option => new GambleOption(option, 0f)).ToList();

            Config.CurrentGamble = new Gamble(gambleName, e.Author.Id, GambleType.Default, options);
            return $"Начата ставка \"{Config.CurrentGamble.GambleName}\"";
        }
    }
}
