using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;

namespace AntiClownDiscordBotVersion2.Models.SlotMachine
{
    public class CircularReel
    {
        public List<ISlotCell> Cells { get; } = new();
        public int CellCount => Cells.Count;

        public CircularReel(IRandomizer randomizer, params CellSeries[] cellList)
        {
            foreach (var cellSeries in cellList)
            {
                for (var i = 0; i < cellSeries.CellCount; i++)
                {
                    Cells.Add(cellSeries.Cell);
                }
            }

            Cells = Cells.Shuffle(randomizer).ToList();
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