using System.Collections.Generic;

namespace AntiClownBot.Models.SlotMachine
{
    public class SlotMachineResult
    {
        public List<ISlotCell> Cells { get; }
        public int Win { get; }

        public SlotMachineResult(List<ISlotCell> cells, int win)
        {
            Cells = cells;
            Win = win;
        }
    }
}