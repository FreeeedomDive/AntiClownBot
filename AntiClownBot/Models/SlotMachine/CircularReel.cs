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

            Cells = Cells.Shuffle().ToList();
        }
        
        public ISlotCell GetCell(int pos)
        {
            var realPos = pos < 0 
                ? CellCount + pos 
                : pos % CellCount;
            return Cells[realPos];
        }
    }
}