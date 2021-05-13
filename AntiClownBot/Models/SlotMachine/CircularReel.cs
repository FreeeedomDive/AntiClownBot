using System;
using System.Collections.Generic;
using System.Linq;

namespace AntiClownBot.Models.SlotMachine
{
    public class CircularReel
    {
        public List<ISlotCell> Cells { get; } = new();
        public int CellCount => Cells.Count;

        public CircularReel(params CellSeries[] cellList)
        {
            foreach (var cellSeries in cellList)
            {
                for (var i = 0; i < cellSeries.CellCount; i++)
                {
                    Cells.Add(cellSeries.Cell);
                }
            }
        }
        
        public ISlotCell GetCell(int pos)
        {
            return Cells[pos];
        }
    }
}