using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Models.SlotMachine
{
    public class CherrySlotCell : ISlotCell
    {
        public CherrySlotCell(int x1Multiplier, int x2Multiplier, int x3Multiplier, DiscordEmoji emoji)
        {
            X1Multiplier = x1Multiplier;
            X2Multiplier = x2Multiplier;
            X3Multiplier = x3Multiplier;
            DiscordEmoji = emoji;
            Emoji = emoji.Name;
        }

        public string Description() => DiscordEmoji + ": " + X1Multiplier + "x  "
                                       + DiscordEmoji + DiscordEmoji + ": " + X2Multiplier + "x  "
                                       + DiscordEmoji + DiscordEmoji + DiscordEmoji + ": " + X3Multiplier + "x";

        public int X1Multiplier { get; }
        public int X2Multiplier { get; }
        public int X3Multiplier { get; }
        public DiscordEmoji DiscordEmoji { get; }
        public string Emoji { get; }
    }
}