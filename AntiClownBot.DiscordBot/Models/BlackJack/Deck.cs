using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;

namespace AntiClownDiscordBotVersion2.Models.BlackJack
{
    public enum Card
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public class Deck
    {
        public Deck(IRandomizer randomizer)
        {
            this.randomizer = randomizer;
        }

        public Deck Init()
        {
            var tempList = new List<Card>
            {
                Card.Two, Card.Three, Card.Four, Card.Five,
                Card.Six, Card.Seven, Card.Eight, Card.Nine, Card.Ten, Card.Jack, Card.Queen, Card.King, Card.Ace
            };

            var list = new List<Card>();
            foreach (var card in tempList)
            {
                for (var count = 0; count < 4; count++)
                {
                    list.Add(card);
                }
            }

            Cards = list.Shuffle(randomizer).ToList();
            return this;
        }
        
        private readonly IRandomizer randomizer;
        public List<Card> Cards;
    }
}