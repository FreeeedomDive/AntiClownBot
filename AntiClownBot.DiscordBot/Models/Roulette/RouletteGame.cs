namespace AntiClownDiscordBotVersion2.Models.Roulette
{
    public class RouletteGame
    {
        private readonly Dictionary<RoulettePlayer, List<Bet>> usersBets = new();
        private readonly object usersBetLocker = new();
        private readonly Random random = new();
        
        public void Bet(RoulettePlayer player, Bet bet)
        {
            if (!usersBets.ContainsKey(player))
            {
                lock (usersBetLocker)
                {
                    if (!usersBets.ContainsKey(player))
                    {
                        usersBets.Add(player, new List<Bet> { bet });
                        return;
                    }
                }
            }
            
            usersBets[player].Add(bet);
        }

        public PlayResult Play()
        {
            var winPoints = new Dictionary<RoulettePlayer, int>();
            var allBetsForUser = new Dictionary<RoulettePlayer, int>();
            var winSector = new Sector(random.Next(0, 37));
            
            foreach (var (player, bets) in usersBets)
            {
                var resultWin = bets.Sum(bet => GetWinByBet(bet, winSector));
                winPoints.Add(player, resultWin);

                if (!allBetsForUser.ContainsKey(player))
                    allBetsForUser.Add(player, 0);
                
                allBetsForUser[player] += bets.Sum(b => b.Points);
            }
            var result = new PlayResult(winPoints, winSector, allBetsForUser);
            
            usersBets.Clear();

            return result;
        }

        private static int GetWinByBet(Bet bet, Sector winSector)
        {
            return bet.Type switch
            {
                BetType.None => throw new ArgumentException("ты чё за говно поставил блять"),
                BetType.Single => bet.SectorForSingle == winSector.Number ? bet.Points * 36 : 0,
                BetType.Red => winSector.Color == Color.Red ? bet.Points * 2 : 0,
                BetType.Black => winSector.Color == Color.Black ? bet.Points * 2 : 0,
                BetType.Even => winSector.IsEven ? bet.Points * 2 : 0,
                BetType.Odd => !winSector.IsEven ? bet.Points * 2 : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}