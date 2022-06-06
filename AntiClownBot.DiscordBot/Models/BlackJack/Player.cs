using AntiClownDiscordBotVersion2.Models.BlackJack;

namespace AntiClownBot.Models.BlackJack
{
    public class Player
    {
        public ulong UserId;
        public string Name;
        public int Value;
        public bool DidHit;
        public bool IsDouble;
        public bool IsBlackJack;
        public Card ReservedCard;
        public bool IsDealer;
    }
}
