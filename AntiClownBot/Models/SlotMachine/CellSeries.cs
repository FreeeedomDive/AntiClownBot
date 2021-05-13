using System;

namespace AntiClownBot.Models.SlotMachine
{
    public class CellSeries
    {
        public int CellCount { get; }
        public ISlotCell Cell { get; }

        public CellSeries(int cellCount, ISlotCell cell)
        {
            CellCount = cellCount;
            Cell = cell;
        }
    }
}