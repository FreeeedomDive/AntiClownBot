namespace AntiClownBot.Models.SlotMachine
{
    public class CherrySlotCell : ISlotCell
    {
        public int X1Multiplier { get; }
        public int X2Multiplier { get; }
        public int X3Multiplier { get; }
        public string Emoji { get; }
        
        public string Description() => Utility.StringEmoji(Emoji) + ": " + X1Multiplier + "x  " + 
                                       Utility.StringEmoji(Emoji) + Utility.StringEmoji(Emoji) + ": " + X2Multiplier + "x  " +
                                       Utility.StringEmoji(Emoji) + Utility.StringEmoji(Emoji) + Utility.StringEmoji(Emoji) + ": " + X3Multiplier + "x";

        public CherrySlotCell(int x1Multiplier, int x2Multiplier, int x3Multiplier, string emoji)
        {
            X1Multiplier = x1Multiplier;
            X2Multiplier = x2Multiplier;
            X3Multiplier = x3Multiplier;
            Emoji = emoji;
        }
    }
}