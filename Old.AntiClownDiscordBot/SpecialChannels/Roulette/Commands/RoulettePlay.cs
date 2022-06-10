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

        public string Execute(MessageCreateEventArgs e)
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
                var member = e.Guild.GetMemberAsync(player.Id).Result;
                message.Append('\n')
                    .Append(member.Nickname)
                    .Append(": ")
                    .Append(winPoints);

                var resultWinPoints =
                    -playResult.Bets.FirstOrDefault(b => b.Key.Equals(player)).Value + winPoints;

                Config.ChangeBalance(player.Id, resultWinPoints, "Рулетка");
            }

            return message.ToString();
        }
    }
}
