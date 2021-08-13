namespace Roulette
{
    public enum BetType
    {
        None,
        Single,
        Red,
        Black,
        Even,
        Odd,
    }
    
    public class Bet
    {
        public int Points { get; set; }
        public BetType Type { get; set; }
        public int SectorForSingle { get; set; }
    }
}