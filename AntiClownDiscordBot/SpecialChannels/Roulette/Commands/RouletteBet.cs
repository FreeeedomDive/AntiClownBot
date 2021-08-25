using DSharpPlus;
using DSharpPlus.EventArgs;
using Roulette;

namespace AntiClownBot.SpecialChannels.Roulette.Commands
{
    public class RouletteBet : ICommand
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;

        public RouletteBet(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
        }

        public string Name => "bet";

        public string Execute(MessageCreateEventArgs e)
        {
            var player = new RoulettePlayer(e.Author.Id);

            var bet = GetBetByMessage(e.Message.Content, e.Author.Id);
            if (bet is null)
            {
                return "чел ты хуйню написал";
            }

            Config.Roulette.Bet(player, bet);
            var messageText = bet.Type == BetType.Single
                ? $"поставлена ставка в {bet.Points} на то что выпадет {bet.SectorForSingle}"
                : $"поставлена ставка в {bet.Points} на то что выпадет {bet.Type}";

            return messageText;
        }

        private Bet GetBetByMessage(string message, ulong userId)
        {
            var splitMessage = message.Split(' ');

            if (splitMessage.Length < 3) return null;
            if (!int.TryParse(splitMessage[1], out var betCount)) return null;
            if (!ParseBetType(splitMessage[2], out var betType)) return null;
            if (betCount <= 0 || betCount > Configuration.GetUserBalance(userId)) return null;

            var bet = new Bet()
            {
                Points = betCount,
                Type = betType,
            };

            if (betType != BetType.Single) return bet;

            if (splitMessage.Length < 4 || !int.TryParse(splitMessage[3], out var sectorNumber) || sectorNumber > 36)
                return null;

            bet.SectorForSingle = sectorNumber;

            return bet;
        }

        private bool ParseBetType(string betType, out BetType type)
        {
            switch (betType)
            {
                case "single":
                    type = BetType.Single;
                    return true;
                case "odd":
                    type = BetType.Odd;
                    return true;
                case "even":
                    type = BetType.Even;
                    return true;
                case "red":
                    type = BetType.Red;
                    return true;
                case "black":
                    type = BetType.Black;
                    return true;
                default:
                    type = BetType.None;
                    return false;
            }
        }
    }
}