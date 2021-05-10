using DSharpPlus;
using DSharpPlus.EventArgs;
using Roulette;

namespace AntiClownBot.Commands.Roulette
{
    public class BetRouletteCommand : BaseCommand
    {
        public BetRouletteCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var player = new RoulettePlayer(user.DiscordId);

            var bet = GetBetByMessage(e.Message.Content);
            if (bet is null)
            {
                await e.Message.RespondAsync("чел ты хуйню написал");
                return;
            }
            
            Config.Roulette.Bet(player, bet);
            var messageText = bet.Type == BetType.Single 
                ? $"поставлена ставка в {bet.Points} на то что выпадет {bet.SectorForSingle}" 
                : $"поставлена ставка в {bet.Points} на то что выпадет {bet.Type}";
            
            await e.Message.RespondAsync(messageText);
        }

        public override string Help()
        {
            return "[размер ставки] [single|red|black|odd|even] [номер сектора если ставка single]";
        }

        private Bet GetBetByMessage(string message)
        {
            var splitMessage = message.Split(' ');

            if (splitMessage.Length < 3) return null;
            if (!int.TryParse(splitMessage[1], out var betCount)) return null;
            if (!ParseBetType(splitMessage[2], out var betType)) return null;

            var bet = new Bet()
            {
                Points = betCount,
                Type = betType,
            };

            if (betType != BetType.Single) return bet;
            
            if (splitMessage.Length < 4 || !int.TryParse(splitMessage[3], out var sectorNumber))
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
                    type =  BetType.Odd;
                    return true;
                case "even":
                    type =  BetType.Even;
                    return true;
                case "red":
                    type =  BetType.Red;
                    return true;
                case "black":
                    type =  BetType.Black;
                    return true;
                default:
                    type = BetType.None;
                    return false;
            }
        }
    }
}