﻿using System.Text;
using AntiClownApiClient;
using AntiClownBot;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using AntiClownDiscordBotVersion2.UserBalance;

namespace AntiClownDiscordBotVersion2.Models.Gamble
{
    public enum GambleBetOptions
    {
        OptionDoesntExist,
        NotEnoughRating,
        SuccessfulBet,
        SuccessfulRaise
    }

    public enum GambleBetResult
    {
        OptionDoesntExist,
        UserIsNotAuthor,
        SuccessfulResult
    }

    public enum GambleType
    {
        Default,
        WithCustomRatio
    }

    public class Gamble
    {
        public Gamble(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IUserBalanceService userBalanceService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.userBalanceService = userBalanceService;
        }

        public Gamble Create(string name, ulong creatorId, GambleType type, IEnumerable<GambleOption> options)
        {
            IsActive = true;
            GambleName = name ?? "";
            CreatorId = creatorId;
            Type = type;
            Bids = options != null
                ? options.ToDictionary(
                    option => option,
                    _ => new List<GambleUser>())
                : new Dictionary<GambleOption, List<GambleUser>>();

            return this;
        }

        public async Task<GambleBetOptions> MakeBid(ulong userId, string option, int bet)
        {
            var user = await apiClient.Users.RatingAsync(userId);

            var options = Bids.Keys.Where(o => o.Name == option).ToList();
            if (options.Count == 0) return GambleBetOptions.OptionDoesntExist;
            var selectedOption = options[0];

            var gamblers = Bids[selectedOption].Where(gambleUser => gambleUser.DiscordId == userId).ToList();
            if (gamblers.Count == 0)
            {
                var gambleUser = new GambleUser(userId);
                if (bet > user.ScamCoins) return GambleBetOptions.NotEnoughRating;
                Bids[selectedOption].Add(gambleUser);
                gambleUser.IncreaseBet(bet);
                return GambleBetOptions.SuccessfulBet;
            }
            else
            {
                var gambleUser = gamblers.First();
                if (bet + gambleUser.Bet > user.ScamCoins) return GambleBetOptions.NotEnoughRating;
                gambleUser.IncreaseBet(bet);
                return GambleBetOptions.SuccessfulRaise;
            }
        }

        public async Task<GambleBetResult> MakeGambleResult(ulong id, string[] correctOptions)
        {
            if (CreatorId != id)
            {
                return GambleBetResult.UserIsNotAuthor;
            }

            if (correctOptions.Where(option =>
                {
                    var bidOptions = Bids.Keys.Where(o => o.Name == option).ToList();
                    if (bidOptions.Count == 0) return true;
                    var selectedOption = bidOptions[0];
                    return !Bids.ContainsKey(selectedOption);
                }).ToList().Count != 0)
                return GambleBetResult.OptionDoesntExist;

            if (Type == GambleType.WithCustomRatio)
            {
                return await MakeCustomGambleResult(correctOptions);
            }

            var incorrectUsers = Bids.Keys
                .Where(key => !correctOptions.Contains(key.Name))
                .SelectMany(key => Bids[key]).ToList();
            var totalIncorrectPoints = incorrectUsers.Select(user => user.Bet).Sum();
            var correctUsers = Bids.Keys
                .Where(key => correctOptions.Contains(key.Name))
                .SelectMany(key => Bids[key]).ToList();
            var totalCorrectPoints = correctUsers.Select(user => user.Bet).Sum();

            if (totalCorrectPoints + totalIncorrectPoints == 0)
            {
                Result = "Ну и нахуй вообще нужна была такая ставка, если все пусси?";
                return GambleBetResult.SuccessfulResult;
            }

            var sb = new StringBuilder();
            sb.Append($"Результаты ставки \"{GambleName}\"\n");

            if (totalCorrectPoints == 0)
            {
                foreach (var incorrectUser in incorrectUsers)
                {
                    var member = await discordClientWrapper.Members.GetAsync(incorrectUser.DiscordId);
                    await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(incorrectUser.DiscordId, -incorrectUser.Bet, "Проигрыш в ставке");
                    sb.Append($"{member.Nickname}: -{incorrectUser.Bet}\n");
                }

                sb.Append("Все проебали свои пойнты, грац, долбаебы");
                Result = sb.ToString();
                return GambleBetResult.SuccessfulResult;
            }

            var ratio = (double)(totalIncorrectPoints + totalCorrectPoints) / totalCorrectPoints;
            foreach (var correctUser in correctUsers)
            {
                var member = await discordClientWrapper.Members.GetAsync(correctUser.DiscordId);
                var win = (int)Math.Floor(correctUser.Bet * ratio - correctUser.Bet);
                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(correctUser.DiscordId, win, "Победа в ставке");
                sb.Append($"{member.Nickname}: +{win}\n");
            }

            foreach (var incorrectUser in incorrectUsers)
            {
                var member = await discordClientWrapper.Members.GetAsync(incorrectUser.DiscordId);
                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(incorrectUser.DiscordId, incorrectUser.Bet, "Проигрыш в ставке");
                sb.Append($"{member.Nickname}: -{incorrectUser.Bet}\n");
            }

            sb.Append($"Всего проебано {totalIncorrectPoints}");
            Result = sb.ToString();
            return GambleBetResult.SuccessfulResult;
        }

        private async Task<GambleBetResult> MakeCustomGambleResult(string[] correctStringOptions)
        {
            var sb = new StringBuilder();
            sb.Append($"Результаты ставки \"{GambleName}\"\n");

            var correctOptions = Bids.Keys
                .Where(key => correctStringOptions.Contains(key.Name))
                .ToList();
            var incorrectOptions = Bids.Keys
                .Where(key => !correctStringOptions.Contains(key.Name))
                .ToList();

            var pointsWon = 0;
            foreach (var correctOption in correctOptions)
            {
                foreach (var correctUser in Bids[correctOption])
                {
                    var member = await discordClientWrapper.Members.GetAsync(correctUser.DiscordId);
                    var win = (int)Math.Floor(correctUser.Bet * correctOption.Ratio - correctUser.Bet);
                    pointsWon += win;
                    await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(correctUser.DiscordId, win, "Победа в ставке");
                    sb.Append($"{member.Nickname}: +{win}\n");
                }
            }

            var pointsLost = 0;

            foreach (var incorrectOption in incorrectOptions)
            {
                foreach (var incorrectUser in Bids[incorrectOption])
                {
                    var member = await discordClientWrapper.Members.GetAsync(incorrectUser.DiscordId);
                    pointsLost += incorrectUser.Bet;
                    await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(incorrectUser.DiscordId, -incorrectUser.Bet, "Победа в ставке");
                    sb.Append($"{member.Nickname}: -{incorrectUser.Bet}\n");
                }
            }

            if (correctOptions.Count == 0)
            {
                sb
                    .Append("Все проебали свои пойнты, грац, долбаебы\n")
                    .Append($"Всего проебано {pointsLost}");
            }

            if (incorrectOptions.Count == 0)
            {
                sb
                    .Append("Все выиграли пойнты, грац\n")
                    .Append($"Всего выиграно {pointsWon}");
            }
            else
            {
                sb
                    .Append($"Всего выиграно {pointsWon}\n")
                    .Append($"Всего проебано {pointsLost}");
            }

            Result = sb.ToString();
            return GambleBetResult.SuccessfulResult;
        }

        public void CloseGamble() => IsActive = false;

        public override string ToString() =>
            $"Текущая ставка:\n{GambleName}\nВарианты:\n" +
            $"{string.Join("\n", Bids.Keys.Select(key => Type == GambleType.Default ? $"\t{key.Name}" : $"\t{key.Name} {key.Ratio:n3}"))}";

        public bool IsActive { get; private set; }

        public string GambleName;
        public ulong CreatorId;
        public Dictionary<GambleOption, List<GambleUser>> Bids;
        public string Result = "Не завершено";
        public GambleType Type;

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IUserBalanceService userBalanceService;
    }
}