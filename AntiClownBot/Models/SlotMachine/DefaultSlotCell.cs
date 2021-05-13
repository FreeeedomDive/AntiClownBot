namespace AntiClownBot.Models.SlotMachine
{
    public class DefaultSlotCell : ISlotCell
    {
        public int Multiplier { get; }
        public string Emoji { get; }

        public string Description() => Utility.StringEmoji(Emoji) + ": " + Multiplier;

        public DefaultSlotCell(int multiplier, string emoji)
        {
            Multiplier = multiplier;
            Emoji = emoji;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ISlotCell);
        }

        public bool Equals(ISlotCell cell)
        {
            return cell != null && Emoji.Equals(cell.Emoji);
        }
        
        public override int GetHashCode()
        {
            return Emoji.GetHashCode();
        }
    }
}