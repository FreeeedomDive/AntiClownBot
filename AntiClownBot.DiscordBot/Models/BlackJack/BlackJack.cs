using System.Text;
using System.Timers;
using AntiClownApiClient;
using AntiClownBot.Models.BlackJack;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using Timer = System.Timers.Timer;

namespace AntiClownDiscordBotVersion2.Models.BlackJack
{
    public enum ResultStatus
    {
        Ok,
        NextPlayer
    }

    public class Result
    {
        public string Message;
        public Card TakenCard;
        public ResultStatus Status;
    }

    public class BlackJack
    {
        public BlackJack(
            IDiscordClientWrapper discordClientWrapper,
            IEmotesProvider emotesProvider,
            IApiClient apiClient,
            IRandomizer randomizer,
            IUserBalanceService userBalanceService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.emotesProvider = emotesProvider;
            this.apiClient = apiClient;
            this.randomizer = randomizer;
            this.userBalanceService = userBalanceService;

            var dealers = new[]
            {
                "John Cock", "Hooba Booba", "Adam Yeppers", "James Poggers", "Johnny PauseChamp", "Nikita Mazes🅱️in"
            };
            Players = new Queue<Player>();
            Players.Enqueue(new Player
            {
                Name = dealers[randomizer.GetRandomNumberBetween(0, dealers.Length)], Value = 0, IsDealer = true
            });
            IsActive = false;
            timer = new Timer(60 * 1000);
            timer.Elapsed += Kick;
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        private void Kick(object? sender, ElapsedEventArgs e)
        {
            Kick().GetAwaiter().GetResult();
        }

        private async Task Kick()
        {
            var player = Players.Dequeue();
            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, -50, "Кик за бездействие в блекджеке");
            var message = $"{player.Name} исключён за бездействие и теряет 50 ScamCoins\n";

            if (Players.First().IsDealer)
                message += MakeResult();
            else message += $"{Players.First().Name}, твой ход.";
            await discordClientWrapper.Messages.SendAsync(843065708023382036, message);
        }

        public async Task<string> Join(ulong userId)
        {
            var member = await discordClientWrapper.Members.GetAsync(userId);
            Players.Enqueue(new Player { UserId = userId, Value = 0, Name = member.ServerOrUserName() });
            return $"{member.ServerOrUserName()} присоединился к игре {await emotesProvider.GetEmoteAsTextAsync("peepoArrive")}";
        }

        public async Task<string> Leave(ulong userId)
        {
            if (Players.First().UserId == userId)
            {
                StopTimer();
                if (IsActive)
                {
                    await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(userId, -50, "Выход из активной игры блекджека");
                }

                var p = Players.Dequeue();
                if (IsActive) StartTimer();
                return $"{p.Name} вышел из игры {await emotesProvider.GetEmoteAsTextAsync("peepoLeave")}\n";
            }

            var potentialRemovableUser = Players.Where(p => userId.Equals(p.UserId)).ToList();
            if (potentialRemovableUser.Count == 0)
                return "Ты и так не участвовал в игре";
            var player = potentialRemovableUser[0];
            Players = Players.WithoutItem(player);
            if (IsActive)
            {
                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(
                    player.UserId,
                    player.IsDouble ? -100 : -50,
                    "Выход из активной игры блекджека");
            }

            return $"{player.Name} вышел из игры {await emotesProvider.GetEmoteAsTextAsync("peepoLeave")}\n";
        }

        public Result GetCard(bool isDouble, Player player)
        {
            var card = CurrentDeck.Cards[^1];
            if (card == Card.Ace && player.Value > 10)
            {
                player.Value += 1;
            }
            else
            {
                player.Value += CurrentDeck.Cards.Last().ToInt();
            }

            CurrentDeck.Cards.RemoveAt(CurrentDeck.Cards.Count - 1);
            var result = new Result
            {
                TakenCard = card
            };
            if (isDouble)
            {
                player.IsDouble = true;
                result.Message = $"{player.Name} удвоил ставку и получил {card}, {player.Value} очков";
                result.Status = ResultStatus.NextPlayer;
            }
            else
            {
                result.Message = $"{player.Name} получил {card}, {player.Value} очков";
                result.Status = player.Value >= 21 ? ResultStatus.NextPlayer : ResultStatus.Ok;
            }

            return result;
        }

        public async Task<string> StartRound()
        {
            if (Players.Count < 2)
            {
                return $"А почему дилер играет один??? {await emotesProvider.GetEmoteAsTextAsync("weirdChamp")}";
            }

            CurrentDeck = new Deck(randomizer).Init();

            var stringBuilder = new StringBuilder();

            var tempQueueWithoutPoorPlayers = new Queue<Player>();
            foreach (var player in Players)
            {
                var userBalance = await apiClient.Users.GetUserBalanceAsync(player.UserId);
                if (!player.IsDealer && userBalance < 50)
                {
                    stringBuilder.Append(
                        $"{player.Name} оказался слишком бедным и кикнут из этой игры\n");
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
                stringBuilder.Append(
                    $"Все оказались слишком бедными, и я остался один {await emotesProvider.GetEmoteAsTextAsync("BibleThump")}");
                return stringBuilder.ToString();
            }

            var dealer = Players.Dequeue();
            var firstCard = GetCard(false, dealer);
            stringBuilder.Append(firstCard.Message + "\n");
            dealer.ReservedCard = CurrentDeck.Cards.Last();
            stringBuilder.Append(
                $"{dealer.Name} получил вторую карту, но какую же... {await emotesProvider.GetEmoteAsTextAsync("monkaHmm")}\n");
            CurrentDeck.Cards.RemoveAt(CurrentDeck.Cards.Count - 1);
            foreach (var player in Players)
            {
                firstCard = GetCard(false, player);
                var secondCard = GetCard(false, player);
                stringBuilder.Append(firstCard.Message + "\n");
                stringBuilder.Append(secondCard.Message + "\n");
                if (firstCard.TakenCard.ToInt() + secondCard.TakenCard.ToInt() != 21)
                    continue;
                player.IsBlackJack = true;
                stringBuilder.Append("BlackJack\n");
            }

            Players.Enqueue(dealer);
            stringBuilder.Append($"{Players.Peek().Name} твой ход");
            IsActive = true;
            return stringBuilder.ToString();
        }

        public async Task<string> MakeResult()
        {
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
                        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, -100, "Проигрыш с double в блекджек");
                        strBuilder.Append($"{player.Name} с удвоенной ставкой проебал 100 очков\n");
                    }
                    else
                    {
                        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, -50, "Проигрыш в блекджек");
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
                            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, -100, "Проигрыш с double в блекджек");
                            strBuilder.Append($"{player.Name} с удвоенной ставкой проебал 100 очков\n");
                        }
                        else
                        {
                            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, -50, "Проигрыш в блекджек");
                            strBuilder.Append($"{player.Name} проебал 50 очков\n");
                        }
                    }
                }
                else if (player.IsBlackJack)
                {
                    await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, 75, "Выигрыш в блекджек");
                    strBuilder.Append($"{player.Name} выиграл BlackJack и получил 75 очков\n");
                }
                else if (dealer.Value > 21 || player.Value > dealer.Value)
                {
                    if (player.IsDouble)
                    {
                        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, 100, "Выигрыш с double в блекджек");
                        strBuilder.Append($"{player.Name} с удвоенной ставкой выиграл 100 очков\n");
                    }
                    else
                    {
                        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, 50, "Выигрыш в блекджек");
                        strBuilder.Append($"{player.Name} выиграл 50 очков\n");
                    }
                }
                else if (player.Value < dealer.Value)
                {
                    if (player.IsDouble)
                    {
                        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, -100, "Проигрыш с double в блекджек");
                        strBuilder.Append($"{player.Name} с удвоенной ставкой проебал 100 очков\n");
                    }
                    else
                    {
                        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(player.UserId, -50, "Проигрыш в блекджек");
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
                dealer.Value += dealer.ReservedCard.ToInt();
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
                    dealer.Value += card.ToInt();
                }

                strBuilder.Append($"{dealer.Name} получил {card}, {dealer.Value} очков\n");
            }

            return strBuilder.ToString();
        }

        public bool IsActive;
        public Queue<Player> Players;
        public Deck CurrentDeck;

        private readonly Timer timer;
        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IEmotesProvider emotesProvider;
        private readonly IApiClient apiClient;
        private readonly IRandomizer randomizer;
        private readonly IUserBalanceService userBalanceService;
    }
}