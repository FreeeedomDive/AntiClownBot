namespace AntiClownBot.Models.SlotMachine
{
    public interface ISlotCell
    {
        public string Emoji { get; }

        public string Description();
    }
}