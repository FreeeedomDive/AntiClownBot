﻿using System.Collections.Generic;

namespace Roulette
{
    public class PlayResult
    {
        public readonly Dictionary<RoulettePlayer, int> WinPoints;
        public readonly Dictionary<RoulettePlayer, int> Bets;
        public readonly Sector WinSector;

        public PlayResult(Dictionary<RoulettePlayer, int> winPoints, Sector winSector, Dictionary<RoulettePlayer, int> bets)
        {
            WinPoints = winPoints;
            WinSector = winSector;
            Bets = bets;
        }    
    }
}