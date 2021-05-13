using System;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.Models.SlotMachine
{
    public class SlotMachine
    {
        public static readonly ISlotCell[] AllCells = {
            new DefaultSlotCell(3, ":BASED:"),
            new DefaultSlotCell(10, ":popCat:"),
            new DefaultSlotCell(25, ":peepoClap:"),
            new DefaultSlotCell(50, ":ricardoFlick:"),
            new DefaultSlotCell(100, ":BOOBA:"),
            new DefaultSlotCell(300, ":rainbowPls:"),
            new DefaultSlotCell(1000, ":PATREGO:"),
            new CherrySlotCell(2, 4, 10, ":blobDance:"),
        };

        public static readonly CircularReel[] CircularReels = {
            new()
            {
                CellSeries = new[]
                {
                    new CellSeries(6, AllCells[0]),
                    new CellSeries(5, AllCells[1]),
                    new CellSeries(5, AllCells[2]),
                    new CellSeries(3, AllCells[3]),
                    new CellSeries(2, AllCells[4]),
                    new CellSeries(2, AllCells[5]),
                    new CellSeries(1, AllCells[6]),
                    new CellSeries(2, AllCells[7]),
                }
            },
            new()
            {
                CellSeries = new[]
                {
                    new CellSeries(6, AllCells[0]),
                    new CellSeries(7, AllCells[1]),
                    new CellSeries(3, AllCells[2]),
                    new CellSeries(4, AllCells[3]),
                    new CellSeries(2, AllCells[4]),
                    new CellSeries(1, AllCells[5]),
                    new CellSeries(1, AllCells[6]),
                    new CellSeries(2, AllCells[7]),
                }
            },
            new()
            {
                CellSeries = new[]
                {
                    new CellSeries(13, AllCells[0]),
                    new CellSeries(6, AllCells[1]),
                    new CellSeries(2, AllCells[2]),
                    new CellSeries(1, AllCells[3]),
                    new CellSeries(1, AllCells[4]),
                    new CellSeries(1, AllCells[5]),
                    new CellSeries(1, AllCells[6]),
                    new CellSeries(1, AllCells[7]),
                }
            }
        };

        public string Description() => string.Join("\n", AllCells.Select(c => c.Description()));

        private Random randomizer = new();
        
        public SlotMachineResult Play(int bet)
        {
            var cells = GetRandomCells().ToList();
            var win = CalcWin(cells, bet);
            var result = new SlotMachineResult(cells, win);
            return result;
        }

        private static int CalcWin(IEnumerable<ISlotCell> cells, int bet)
        {
            var cellsList = cells.ToList();
            var cherries = cellsList.OfType<CherrySlotCell>().ToList();
            if (cherries.Any())
            {
                return cherries.Count switch
                {
                    1 => bet * cherries[0].X1Multiplier,
                    2 => bet * cherries[0].X2Multiplier,
                    3 => bet * cherries[0].X3Multiplier,
                    _ => throw new ArgumentOutOfRangeException("пиздец чел сдохни")
                };
            }

            if (cellsList.Count(c => c.Equals(cellsList[0])) == 3 && cellsList[0] is DefaultSlotCell cell)
                return cell.Multiplier * bet;

            return 0;
        }

        private IEnumerable<ISlotCell> GetRandomCells()
        {
            return CircularReels
                .Select(reel => new {reel, winPos = randomizer.Next(0, reel.CellSeries.Length)})
                .Select(@t => @t.reel.GetCell(@t.winPos))
                .ToList();
        }
    }
}