using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownBot.Models.BlackJack
{
    public enum GetResultStatus
    {
        Ok,
        NextPlayer
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
        private Configuration _configuration;
        private Timer _timer;
        public BlackJack()
        {
            var tempArray = new[]
                {"John Cock", "Hooba Booba", "Adam Yeppers", "James Poggers", "Johnny PauseChamp", "Nikita Mazes🅱️in"};
            Players = new Queue<Player>();
            Players.Enqueue(new Player
                {Name = tempArray[Randomizer.GetRandomNumberBetween(0, tempArray.Length)], Value = 0, IsDealer = true});
            IsActive = false;
            _timer = new Timer(60 * 1000);
            _timer.Elapsed += Kick;
        }
        public void StartTimer()
        {
            _timer.Start();
        }
        public void StopTimer()
        {
            _timer.Stop();
        }
        private async void Kick(object sender, System.Timers.ElapsedEventArgs e)
        {
            var player = Players.Dequeue();
            _configuration.Users[player.UserId].ChangeRating(-50);
            _configuration.Save();
            var message = $"{player.Name} исключён за бездействие и теряет 50 ClownCoins\n";
            
            if (Players.First().IsDealer)
                message += MakeResult();
            else message += $"{Players.First().Name}, твой ход.";
            await Utility.Client
                .Guilds[277096298761551872]
                .GetChannel(843065708023382036)
                .SendMessageAsync(message);
        }
        public string Join(SocialRatingUser user)
        {
            Players.Enqueue(new Player {UserId = user.DiscordId, Value = 0, Name = user.DiscordUsername});
            return $"{user.DiscordUsername} присоединился к игре {Utility.StringEmoji(":peepoArrive:")}";
        }

        public string Leave(SocialRatingUser user)
        {
            _configuration ??= Configuration.GetConfiguration();
            if(Players.First().UserId == user.DiscordId)
            {
                StopTimer();
                user.ChangeRating(Players.First().IsDouble ? -100 : -50);
                Players.Dequeue();
                StartTimer();
                return $"{user.DiscordUsername} вышел из игры {Utility.StringEmoji(":peepoLeave:")}\n";
            }
            var potentialRemovableUser = Players.Where(p => user.DiscordId.Equals(p.UserId)).ToList();
            if (potentialRemovableUser.Count == 0)
                return "Ты и так не участвовал в игре";
            var player = potentialRemovableUser[0];
            Players = Players.WithoutItem(player);
            if (IsActive)
                _configuration.Users[player.UserId].ChangeRating(player.IsDouble ? -100 : -50);
            return $"{user.DiscordUsername} вышел из игры {Utility.StringEmoji(":peepoLeave:")}\n";
        }

        public GetResult GetCard(bool isDouble, Player player)
        {
            var card = CurrentDeck.Cards[CurrentDeck.Cards.Count - 1];
            if (card == Card.Ace && player.Value > 10)
            {
                player.Value += 1;
            }
            else
            {
                player.Value += Utility.CardToInt(CurrentDeck.Cards.Last());
            }

            CurrentDeck.Cards.RemoveAt(CurrentDeck.Cards.Count - 1);
            var result = new GetResult
            {
                TakenCard = card
            };
            if (isDouble)
            {
                player.IsDouble = true;
                result.Message = $"{player.Name} удвоил ставку и получил {card}, {player.Value} очков";
                result.Status = GetResultStatus.NextPlayer;
            }
            else
            {
                result.Message = $"{player.Name} получил {card}, {player.Value} очков";
                result.Status = player.Value >= 21 ? GetResultStatus.NextPlayer : GetResultStatus.Ok;
            }

            return result;
        }

        public string StartRound()
        {
            _configuration ??= Configuration.GetConfiguration();
            if (Players.Count < 2)
            {
                return $"А почему дилер играет один??? {Utility.StringEmoji(":weirdChamp")}";
            }

            CurrentDeck = new Deck().Init();

            var stringBuilder = new StringBuilder();

            var tempQueueWithoutPoorPlayers = new Queue<Player>();
            foreach (var player in Players)
            {
                if (!player.IsDealer && _configuration.Users[player.UserId].SocialRating < 50)
                {
                    stringBuilder.Append(
                        $"{_configuration.Users[player.UserId].DiscordUsername} оказался слишком бедным и кикнут из этой игры\n");
                    continue;
                }

                tempQueueWithoutPoorPlayers.Enqueue(player);
                player.Value = 0;
                player.IsBlackJack = false;
                player.IsDouble = false;
                player.DidHit = false;
            }

            Players = tempQueueWithoutPoorPlayers;

            if (Players.Count < 2)
            {
                stringBuilder.Append($"Все оказались слишком бедными, и я остался один {Utility.StringEmoji(":BibleThump:")}");
                return stringBuilder.ToString();
            }

            var dealer = Players.Dequeue();
            var firstCard = GetCard(false, dealer);
            stringBuilder.Append(firstCard.Message + "\n");
            dealer.ReservedCard = CurrentDeck.Cards.Last();
            stringBuilder.Append(
                $"{dealer.Name} получил вторую карту, но какую же... {Utility.StringEmoji(":monkaHmm:")}\n");
            CurrentDeck.Cards.RemoveAt(CurrentDeck.Cards.Count - 1);
            foreach (var player in Players)
            {
                firstCard = GetCard(false, player);
                var secondCard = GetCard(false, player);
                stringBuilder.Append(firstCard.Message + "\n");
                stringBuilder.Append(secondCard.Message + "\n");
                if (Utility.CardToInt(firstCard.TakenCard) + Utility.CardToInt(secondCard.TakenCard) != 21)
                    continue;
                player.IsBlackJack = true;
                stringBuilder.Append("BlackJack\n");
            }

            Players.Enqueue(dealer);
            stringBuilder.Append($"{Players.Peek().Name} твой ход");
            IsActive = true;
            return stringBuilder.ToString();
        }

        public string MakeResult()
        {
            _configuration ??= Configuration.GetConfiguration();
            var strBuilder = new StringBuilder();
            var dealer = Players.Dequeue();
            strBuilder.Append(DealerGetLastCards(dealer));
            Players.Enqueue(dealer);
            while (!Players.Peek().IsDealer)
            {
                var player = Players.Dequeue();
                if (player.Value > 21)
                {
                    if (player.IsDouble)
                    {
                        _configuration.Users[player.UserId].ChangeRating(-100);
                        strBuilder.Append($"{player.Name} с удвоенной ставкой проебал 100 очков\n");
                    }
                    else
                    {
                        _configuration.Users[player.UserId].ChangeRating(-50);
                        strBuilder.Append($"{player.Name} проебал 50 очков\n");
                    }
                }
                else if (dealer.IsBlackJack)
                {
                    if (player.IsBlackJack)
                    {
                        strBuilder.Append($"{player.Name} ничья\n");
                    }
                    else
                    {
                        if (player.IsDouble)
                        {
                            _configuration.Users[player.UserId].ChangeRating(-100);
                            strBuilder.Append($"{player.Name} с удвоенной ставкой проебал 100 очков\n");
                        }
                        else
                        {
                            _configuration.Users[player.UserId].ChangeRating(-50);
                            strBuilder.Append($"{player.Name} проебал 50 очков\n");
                        }
                    }
                }
                else if (player.IsBlackJack)
                {
                    _configuration.Users[player.UserId].ChangeRating(75);
                    strBuilder.Append($"{player.Name} выиграл BlackJack и получил 75 очков\n");
                }
                else if (dealer.Value > 21 || player.Value > dealer.Value)
                {
                    if (player.IsDouble)
                    {
                        _configuration.Users[player.UserId].ChangeRating(100);
                        strBuilder.Append($"{player.Name} с удвоенной ставкой выиграл 100 очков\n");
                    }
                    else
                    {
                        _configuration.Users[player.UserId].ChangeRating(50);
                        strBuilder.Append($"{player.Name} выиграл 50 очков\n");
                    }
                }
                else if (player.Value < dealer.Value)
                {
                    if (player.IsDouble)
                    {
                        _configuration.Users[player.UserId].ChangeRating(-100);
                        strBuilder.Append($"{player.Name} с удвоенной ставкой проебал 100 очков\n");
                    }
                    else
                    {
                        _configuration.Users[player.UserId].ChangeRating(-50);
                        strBuilder.Append($"{player.Name} проебал 50 очков\n");
                    }
                }
                else
                {
                    strBuilder.Append($"{player.Name} ничья\n");
                }

                Players.Enqueue(player);
            }

            IsActive = false;
            return strBuilder.ToString();
        }

        public string DealerGetLastCards(Player dealer)
        {
            var strBuilder = new StringBuilder();
            if (dealer.ReservedCard == Card.Ace && dealer.Value > 10)
            {
                dealer.Value += 1;
            }
            else
            {
                dealer.Value += Utility.CardToInt(dealer.ReservedCard);
            }

            strBuilder.Append($"{dealer.Name} получил {dealer.ReservedCard}, {dealer.Value} очков\n");
            if (dealer.Value == 21)
                dealer.IsBlackJack = true;
            while (dealer.Value < 17)
            {
                var card = CurrentDeck.Cards.Last();
                if (card == Card.Ace && dealer.Value > 10)
                {
                    dealer.Value += 1;
                }
                else
                {
                    dealer.Value += Utility.CardToInt(card);
                }

                strBuilder.Append($"{dealer.Name} получил {card}, {dealer.Value} очков\n");
            }

            return strBuilder.ToString();
        }
    }
}