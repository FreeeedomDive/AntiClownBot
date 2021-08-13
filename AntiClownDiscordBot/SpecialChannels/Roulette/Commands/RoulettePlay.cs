using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Emzi0767;

namespace AntiClownBot.SpecialChannels.Roulette.Commands
{
    public class RoulettePlay : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        public RoulettePlay(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }
        public string Name => "play";

        public string Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var playResult = Config.Roulette.Play();
            var message = new StringBuilder();
            message.Append("Выпал номер ")
                .Append(playResult.WinSector.Number)
                .Append(" с цветом ")
                .Append(playResult.WinSector.Color);

            message.Append(playResult.WinPoints.Count == 0
                ? "\nА игроков то и не было"
                : "\nРезультаты стола: ");

            foreach (var (player, winPoints) in playResult.WinPoints)
            {
                Config.Users.TryGetValue(player.Id, out var nick);
                message.Append("\n")
                    .Append(nick?.DiscordUsername)
                    .Append(": ")
                    .Append(winPoints);

                var resultWinPoints =
                    -playResult.Bets.FirstOrDefault(b => b.Key.Equals(player)).Value + winPoints;

                nick.ChangeRating(resultWinPoints);
            }

            return message.ToString();
        }
    }
}
