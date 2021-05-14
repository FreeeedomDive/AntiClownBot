using System;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.Models.SlotMachine
{
    public class SlotMachine
    {
        public static readonly ISlotCell[] AllCells = {
            new DefaultSlotCell(4, ":BASED:"),
            new DefaultSlotCell(15, ":popCat:"),
            new DefaultSlotCell(30, ":peepoClap:"),
            new DefaultSlotCell(50, ":ricardoFlick:"),
            new DefaultSlotCell(100, ":BOOBA:"),
            new DefaultSlotCell(500, ":RainbowPls:"),
            new DefaultSlotCell(1000, ":PATREGO:"),
            new CherrySlotCell(2, 5, 15, ":blobDance:"),
        };

        public static readonly CircularReel[] CircularReels = {
            new(new CellSeries(19, AllCells[0]), //x4
                    new CellSeries(14, AllCells[1]),        //x15
                    new CellSeries(9, AllCells[2]),         //x30
                    new CellSeries(7, AllCells[3]),         //x50
                    new CellSeries(4, AllCells[4]),         //x100
                    new CellSeries(3, AllCells[5]),         //x500
                    new CellSeries(2, AllCells[6]),         //x1000
                    new CellSeries(5, AllCells[7])),        //x2-x5-x15
            new(new CellSeries(16, AllCells[0]), //x4
                    new CellSeries(16, AllCells[1]),        //x15
                    new CellSeries(9, AllCells[2]),         //x30
                    new CellSeries(8, AllCells[3]),         //x50
                    new CellSeries(6, AllCells[4]),         //x100
                    new CellSeries(3, AllCells[5]),         //x500
                    new CellSeries(2, AllCells[6]),         //x1000
                    new CellSeries(5, AllCells[7])),        //x2-x5-x15
            new(new CellSeries(23, AllCells[0]), //x4
                    new CellSeries(10, AllCells[1]),        //x15
                    new CellSeries(8, AllCells[2]),         //x30
                    new CellSeries(5, AllCells[3]),         //x50
                    new CellSeries(7, AllCells[4]),         //x100
                    new CellSeries(3, AllCells[5]),         //x500
                    new CellSeries(2, AllCells[6]),         //x1000
                    new CellSeries(6, AllCells[7]))         //x2-x5-x15
        };

        public static string Description() => "Таблица выплат:\n" + string.Join("\n", AllCells.Select(c => c.Description()));
        
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
                .Select(reel => new {reel, winPos = Randomizer.GetRandomNumberBetween(0, reel.CellCount)})
                .Select(@t => @t.reel.GetCell(@t.winPos))
                .ToList();
        }
    }
}