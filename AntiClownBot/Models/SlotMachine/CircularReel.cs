using System;
using System.Linq;

namespace AntiClownBot.Models.SlotMachine
{
    public class CircularReel
    {
        public CellSeries[] CellSeries { get; set; }
        public int CellCount => CellSeries.Sum(c => c.Count);

        public ISlotCell GetCell(int pos)
        {
            var currentPos = 0;
            
            foreach (var cell in CellSeries)
            {
                if (currentPos + cell.Count < pos)
                {
                    currentPos += cell.Count;
                }
                else
                {
                    return cell.Cell;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}