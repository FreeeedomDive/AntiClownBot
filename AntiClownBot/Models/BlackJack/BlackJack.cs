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
        public List<Player> Players;
        public DiscordMessage BotMessage;
        public Player CurrentPlayer;
        public Deck CurrentDeck;
        public BlackJack()
        {
            var tempArray = new string[] { "John Cock", "Hooba Booba", "Adam Yeppers", "James Poggers", "Johnny PauseChamp" };
            Players = new List<Player> { new Player { Name = tempArray[Randomizer.GetRandomNumberBetween(0, tempArray.Length)] , Value = 0} };
            IsActive = false;
        }
        public string Join(SocialRatingUser user)
        {
            Players.Add(new Player { User = user , Value = 0, NextPlayer = Players.Last(), Name = user.DiscordUsername});
            return $"{user.DiscordUsername} joined BlackJack";
        }
        public GetResult GetCard(bool isDouble)
        {
            var card = CurrentDeck.cards[CurrentDeck.cards.Count - 1];
            if(card == Card.Ace && CurrentPlayer.Value > 10)
            {
                CurrentPlayer.Value += 1;
            }
            else
            {
                CurrentPlayer.Value += (int)CurrentDeck.cards[CurrentDeck.cards.Count - 1];
            }
            CurrentDeck.cards.RemoveAt(CurrentDeck.cards.Count - 1);
            var result = new GetResult();
            result.TakenCard = card;
            if(isDouble)
            {
                CurrentPlayer.IsDouble = true;
                result.Message = $"{CurrentPlayer.Name} doubled and got {card}, {CurrentPlayer.Value} points now";
                result.Status = GetResultStatus.NextPlayer;
                if (CurrentPlayer.Value > 21)
                {
                    result.Message += " :PogOff:\n";
                }
                else if (CurrentPlayer.Value < 17)
                {
                    result.Message += " :pepeLaugh:\n";
                }
                else
                {
                    result.Message += " :Pog:\n";
                }
            }
            else
            {
                result.Message = $"{CurrentPlayer.Name} got {card}, {CurrentPlayer.Value} points now ";
                if (CurrentPlayer.Value == 21)
                {
                    result.Status = GetResultStatus.NextPlayer;
                    result.Message += ":monkaGIGA:\n";
                }
                else if (CurrentPlayer.Value > 21)
                {
                    result.Status = GetResultStatus.NextPlayer;
                    result.Message += ":KEKW:\n";
                }
                else
                {
                    result.Status = GetResultStatus.OK;
                    result.Message += ":pauseChamp:\n";
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
                player.CanDouble = true;
            }
            var stringBuilder = new StringBuilder();
            CurrentPlayer = Players.Last();
            var firstCard = new GetResult();
            var secondCard = new GetResult();
            while (CurrentPlayer.NextPlayer != null)
            {
                firstCard = GetCard(false);
                secondCard = GetCard(false);
                stringBuilder.Append(firstCard.Message + "\n");
                stringBuilder.Append(secondCard.Message + "\n");
                if((int) firstCard.TakenCard + (int) secondCard.TakenCard == 21)
                {
                    CurrentPlayer.IsBlackJack = true;
                    stringBuilder.Append("BlackJack :Pog:\n");
                }
                CurrentPlayer = CurrentPlayer.NextPlayer;
            }
            firstCard = GetCard(false);
            stringBuilder.Append(firstCard.Message + "\n");
            CurrentPlayer.ReservedCard = CurrentDeck.cards.Last();
            stringBuilder.Append($"{CurrentPlayer.Name} got second card, but guess which one :monkaHmm:\n");
            CurrentDeck.cards.RemoveAt(CurrentDeck.cards.Count - 1);
            CurrentPlayer = Players.Last();
            while(CurrentPlayer.IsBlackJack)
            {
                CurrentPlayer = CurrentPlayer.NextPlayer;
            }
            if(CurrentPlayer.NextPlayer == null)
            {
                stringBuilder.Append(MakeResult());
                return stringBuilder.ToString();
            }
            stringBuilder.Append($"@{CurrentPlayer.Name} Your Turn");
            IsActive = true;
            return stringBuilder.ToString();
        }
        public string MakeResult()
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(DealerGetLastCards());
            for(var i = Players.Count - 1; i > 0; i--)
            {
                var value = Players[i].Value;
                if (value > 21)
                {
                    if (Players[i].IsDouble)
                    {
                        Players[i].User.DecreaseRating(100);
                        strBuilder.Append($"{Players[i].Name} with double lost 100 points :Kekega:\n");
                    }
                    else
                    {
                        Players[i].User.DecreaseRating(50);
                        strBuilder.Append($"{Players[i].Name} lost 50 points :KEKW:\n");
                    }
                }
                else if (CurrentPlayer.IsBlackJack)
                {
                    if (Players[i].IsBlackJack)
                    {
                        strBuilder.Append($"{Players[i].Name} tie :PogOff:\n");
                    }
                    else
                    {
                        Players[i].User.DecreaseRating(50);
                        strBuilder.Append($"{Players[i].Name} lost 50 points :KEKW:\n");
                    }
                }
                else if (Players[i].IsBlackJack)
                {
                    Players[i].User.IncreaseRating(75);
                    strBuilder.Append($"{Players[i].Name} won BlackJack and got 75 points :Pog:\n");
                }
                else if (CurrentPlayer.Value > 21 || value > CurrentPlayer.Value)
                {
                    if (Players[i].IsDouble)
                    {
                        Players[i].User.IncreaseRating(100);
                        strBuilder.Append($"{Players[i].Name} won with double and got 100 points :Pog:\n");
                    }
                    else
                    {
                        Players[i].User.IncreaseRating(50);
                        strBuilder.Append($"{Players[i].Name} won and got 50 points\n");
                    }
                }
                else
                {
                    if (Players[i].IsDouble)
                    {
                        Players[i].User.DecreaseRating(100);
                        strBuilder.Append($"{Players[i].Name} with double lost 100 points :Kekega:\n");
                    }
                    else
                    {
                        Players[i].User.DecreaseRating(50);
                        strBuilder.Append($"{Players[i].Name} lost 50 points :KEKW:\n");
                    }
                }
            }
            IsActive = false;
            return strBuilder.ToString();
        }
        public string DealerGetLastCards()
        {
            var strBuilder = new StringBuilder();
            CurrentPlayer = Players.First();
            if (CurrentPlayer.ReservedCard == Card.Ace && CurrentPlayer.Value > 10)
            {
                CurrentPlayer.Value += 1;
            }
            else
            {
                CurrentPlayer.Value += (int)CurrentDeck.cards[CurrentDeck.cards.Count - 1];
            }
            strBuilder.Append($"{CurrentPlayer.Name} got {CurrentPlayer.ReservedCard}, {CurrentPlayer.Value} points now\n");
            if (CurrentPlayer.Value == 21)
                CurrentPlayer.IsBlackJack = true;
            while(CurrentPlayer.Value < 17)
            {
                var card = CurrentDeck.cards.Last();
                CurrentPlayer.Value += (int)card;
                strBuilder.Append($"{CurrentPlayer.Name} got {card}, {CurrentPlayer.Value} points now\n");
            }
            return strBuilder.ToString();
        }
    }
}
