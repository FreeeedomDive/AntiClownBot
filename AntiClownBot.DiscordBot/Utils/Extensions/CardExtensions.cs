using AntiClownBot.Models.BlackJack;
using AntiClownDiscordBotVersion2.Models.BlackJack;

namespace AntiClownDiscordBotVersion2.Utils.Extensions;

public static class CardExtensions
{
    public static int ToInt(this Card card)
    {
        return card switch
        {
            Card.Two => 2,
            Card.Three => 3,
            Card.Four => 4,
            Card.Five => 5,
            Card.Six => 6,
            Card.Seven => 7,
            Card.Eight => 8,
            Card.Nine => 9,
            Card.Ten => 10,
            Card.Jack => 10,
            Card.Queen => 10,
            Card.King => 10,
            Card.Ace => 11,
            _ => throw new ArgumentOutOfRangeException(nameof(card), card, null)
        };
    }
}