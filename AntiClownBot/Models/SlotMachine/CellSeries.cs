namespace AntiClownBot.Models.SlotMachine
{
    public class CellSeries
    {
        public int Count { get; }
        public ISlotCell Cell { get; }

        public CellSeries(int count, ISlotCell cell)
        {
            Count = count;
            Cell = cell;
        }
    }
}