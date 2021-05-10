using System.Collections.Generic;

namespace Roulette
{
    public class PlayResult
    {
        public readonly Dictionary<RoulettePlayer, int> WinPoints;
        public readonly Sector WinSector;

        public PlayResult(Dictionary<RoulettePlayer, int> winPoints, Sector winSector)
        {
            WinPoints = winPoints;
            WinSector = winSector;
        }
    }
}