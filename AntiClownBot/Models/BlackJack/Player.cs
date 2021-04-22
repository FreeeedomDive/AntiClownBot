namespace AntiClownBot.Models.BlackJack
{
    public class Player
    {
        public SocialRatingUser User;
        public string Name;
        public int Value;
        public bool DidHit;
        public bool IsDouble;
        public bool IsBlackJack;
        public Card ReservedCard;
        public bool IsDealer;
    }
}
