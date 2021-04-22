using DSharpPlus;
using System.Net.Sockets;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Models.BlackJack
{
    public enum GetResultStatus
    {
        OK,
        NextPlayer
    }
    public enum StartResultStatus
    {
        OK,
        NoPlayers
    }
    public class StartResult
    {
        public string Message;
        public StartResultStatus Status;
    }
    public class GetResult
    {
        public string Message;
        public Card TakenCard;
        public GetResultStatus Status;
    }
    public class BlackJack
    {
        public bool IsActive;
        public Queue<Player> Players;
        public Deck CurrentDeck;
        public BlackJack()
        {
            var tempArray = new string[] { "John Cock", "Hooba Booba", "Adam Yeppers", "James Poggers", "Johnny PauseChamp" };
            Players = new Queue<Player> ();
            Players.Enqueue(new Player { Name = tempArray[Randomizer.GetRandomNumberBetween(0, tempArray.Length)], Value = 0 , IsDealer = true});
            IsActive = false;
        }
        public string Join(SocialRatingUser user)
        {
            Players.Enqueue(new Player { User = user , Value = 0, Name = user.DiscordUsername});
            return $"{user.DiscordUsername} joined BlackJack";
        }
        public GetResult GetCard(bool isDouble, Player player)
        {
            var card = CurrentDeck.Cards[CurrentDeck.Cards.Count - 1];
            if(card == Card.Ace && player.Value > 10)
            {
                player.Value += 1;
            }
            else
            {
                player.Value += (int)CurrentDeck.Cards[CurrentDeck.Cards.Count - 1];
            }
            CurrentDeck.Cards.RemoveAt(CurrentDeck.Cards.Count - 1);
            var result = new GetResult();
            result.TakenCard = card;
            if(isDouble)
            {
                player.IsDouble = true;
                result.Message = $"{player.Name} doubled and got {card}, {player.Value} points now";
                result.Status = GetResultStatus.NextPlayer;
            }
            else
            {
                result.Message = $"{player.Name} got {card}, {player.Value} points now ";
                if (player.Value >= 21)
                {
                    result.Status = GetResultStatus.NextPlayer;
                }
                else
                {
                    result.Status = GetResultStatus.OK;
                }
            }
            return result;
        }
        public string StartRound()
        {
            if(Players.Count < 2)
            {
                return "Hello, why dealer playing alone?";
            }
            CurrentDeck = new Deck();
            foreach(var player in Players)
            {
                player.Value = 0;
                player.IsBlackJack = false;
                player.IsDouble = false;
                player.DidHit = false;
            }
            var stringBuilder = new StringBuilder();
            var firstCard = new GetResult();
            var secondCard = new GetResult();
            var dealer = Players.Dequeue();
            firstCard = GetCard(false, dealer);
            stringBuilder.Append(firstCard.Message + "\n");
            dealer.ReservedCard = CurrentDeck.Cards.Last();
            stringBuilder.Append($"{dealer.Name} got second card, but guess which one :monkaHmm:\n");
            CurrentDeck.Cards.RemoveAt(CurrentDeck.Cards.Count - 1);
            Players.Enqueue(dealer);
            foreach(var player in Players)
            {
                firstCard = GetCard(false, player);
                secondCard = GetCard(false, player);
                stringBuilder.Append(firstCard.Message + "\n");
                stringBuilder.Append(secondCard.Message + "\n");
                if((int) firstCard.TakenCard + (int) secondCard.TakenCard == 21)
                {
                    player.IsBlackJack = true;
                    stringBuilder.Append("BlackJack\n");
                }
            }
            stringBuilder.Append($"{Players.Peek().Name} Your Turn");
            IsActive = true;
            return stringBuilder.ToString();
        }
        public string MakeResult()
        {
            var strBuilder = new StringBuilder();
            var dealer = Players.Dequeue();
            strBuilder.Append(DealerGetLastCards(dealer));
            Players.Enqueue(dealer);
            while(!Players.Peek().IsDealer)
            {
                var player = Players.Dequeue();
                if (player.Value > 21)
                {
                    if (player.IsDouble)
                    {
                        player.User.DecreaseRating(100);
                        strBuilder.Append($"{player.Name} with double lost 100 points\n");
                    }
                    else
                    {
                        player.User.DecreaseRating(50);
                        strBuilder.Append($"{player.Name} lost 50 points\n");
                    }
                }
                else if (dealer.IsBlackJack)
                {
                    if (player.IsBlackJack)
                    {
                        strBuilder.Append($"{player.Name} tie\n");
                    }
                    else
                    {
                        if (player.IsDouble)
                        {
                            player.User.DecreaseRating(100);
                            strBuilder.Append($"{player.Name} with double lost 100 points\n");
                        }
                        else
                        {
                            player.User.DecreaseRating(50);
                            strBuilder.Append($"{player.Name} lost 50 points\n");
                        }
                    }
                }
                else if (player.IsBlackJack)
                {
                    if(dealer.Value != 21)
                    {
                        Players.Peek().User.IncreaseRating(75);
                        strBuilder.Append($"{Players.Peek().Name} won BlackJack and got 75 points\n");
                    }
                    else
                    {
                        strBuilder.Append($"{player.Name} tie\n");
                    }
                }
                else if (dealer.Value > 21 || player.Value > Players.Peek().Value)
                {
                    if (player.IsDouble)
                    {
                        player.User.IncreaseRating(100);
                        strBuilder.Append($"{player.Name} won with double and got 100 points\n");
                    }
                    else
                    {
                        player.User.IncreaseRating(50);
                        strBuilder.Append($"{player.Name} won and got 50 points\n");
                    }
                }
                else
                {
                    if (player.IsDouble)
                    {
                        player.User.DecreaseRating(100);
                        strBuilder.Append($"{player.Name} with double lost 100 points\n");
                    }
                    else
                    {
                        player.User.DecreaseRating(50);
                        strBuilder.Append($"{player.Name} lost 50 points\n");
                    }
                }
                Players.Enqueue(player);
            }
            IsActive = false;
            return strBuilder.ToString();
        }
        public string DealerGetLastCards(Player dealer)
        {
            var strBuilder = new StringBuilder();
            if (dealer.ReservedCard == Card.Ace && Players.Peek().Value > 10)
            {
                dealer.Value += 1;
            }
            else
            {
                dealer.Value += (int) dealer.ReservedCard;
            }
            strBuilder.Append($"{dealer.Name} got {dealer.ReservedCard}, {dealer.Value} points now\n");
            if (dealer.Value == 21)
                dealer.IsBlackJack = true;
            while(dealer.Value < 17)
            {
                var card = CurrentDeck.Cards.Last();
                dealer.Value += (int)card;
                strBuilder.Append($"{dealer.Name} got {card}, {dealer.Value} points now\n");
            }
            return strBuilder.ToString();
        }
    }
}
