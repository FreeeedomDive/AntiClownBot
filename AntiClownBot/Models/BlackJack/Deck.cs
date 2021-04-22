using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Models.BlackJack
{
    public enum Card
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 10,
        Queen = 10,
        King = 10,
        Ace = 11

    }
    public class Deck
    {
        public readonly List<Card> Cards;
        public Deck()
        {
            var tempList = new List<Card> { Card.Two, Card.Three, Card.Four, Card.Five, 
                Card.Six, Card.Seven, Card.Eight, Card.Nine, Card.Ten, Card.Jack, Card.Queen, Card.King, Card.Ace };
            
            var list = new List<Card>();
            foreach(var card in tempList)
            {
                for(var count = 0; count < 8; count++)
                {
                    list.Add(card);
                }
            }

            Cards = list.Shuffle().ToList();
        }
    }
}
