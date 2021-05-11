using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Emzi0767;

namespace AntiClownBot.Commands.Roulette
{
    public class PlayRouletteCommand : BaseCommand
    {
        public PlayRouletteCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
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
                
                if (resultWinPoints >= 0)
                    Utility.IncreaseRating(Config, nick, resultWinPoints, e);
                else
                    Utility.DecreaseRating(Config, nick, -resultWinPoints, e);
            }
            
            await e.Message.RespondAsync(message.ToString());
        }

        public override string Help()
        {
            return "Команда чтобы запустить рулетку и раздать очки";
        }
    }
}