using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Models.SlotMachine
{
    public class DefaultSlotCell : ISlotCell
    {
        public DefaultSlotCell(int multiplier, DiscordEmoji emoji)
        {
            Multiplier = multiplier;
            DiscordEmoji = emoji;
            Emoji = emoji.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ISlotCell);
        }

        public bool Equals(ISlotCell? cell)
        {
            return cell != null && Emoji.Equals(cell.Emoji);
        }

        public override int GetHashCode()
        {
            return Emoji.GetHashCode();
        }

        public int Multiplier { get; }
        public DiscordEmoji DiscordEmoji { get; }
        public string Emoji { get; }

        public string Description() => DiscordEmoji + " " +
                                       DiscordEmoji + " " +
                                       DiscordEmoji + ": " + DiscordEmoji + "x";
    }
}