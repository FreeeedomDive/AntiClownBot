using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntiClownBot.Models.Gamble;

namespace AntiClownBot
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
        public string GambleName;
        public ulong CreatorId;
        public Dictionary<GambleOption, List<GambleUser>> Bids;
        public string Result = "Не завершено";
        public GambleType Type;

        public bool IsActive { get; private set; }

        public Gamble(string name, ulong creatorId, GambleType type, IEnumerable<GambleOption> options)
        {
            IsActive = true;
            GambleName = name ?? "";
            CreatorId = creatorId;
            Type = type;
            Bids = options != null
                ? options.ToDictionary(
                    option => option,
                    option => new List<GambleUser>())
                : new Dictionary<GambleOption, List<GambleUser>>();
        }

        public GambleBetOptions MakeBid(SocialRatingUser user, string option, int bet)
        {
            var options = Bids.Keys.Where(o => o.Name == option).ToList();
            if (options.Count == 0) return GambleBetOptions.OptionDoesntExist;
            var selectedOption = options[0];

            var gamblers = Bids[selectedOption].Where(gambleUser => gambleUser.DiscordId == user.DiscordId).ToList();
            if (gamblers.Count == 0)
            {
                var gambleUser = new GambleUser(user.DiscordId);
                if (bet > user.SocialRating) return GambleBetOptions.NotEnoughRating;
                Bids[selectedOption].Add(gambleUser);
                gambleUser.IncreaseBet(bet);
                return GambleBetOptions.SuccessfulBet;
            }
            else
            {
                var gambleUser = gamblers.First();
                if (bet + gambleUser.Bet > user.SocialRating) return GambleBetOptions.NotEnoughRating;
                gambleUser.IncreaseBet(bet);
                return GambleBetOptions.SuccessfulRaise;
            }
        }

        public GambleBetResult MakeGambleResult(ulong id, string[] correctOptions)
        {
            var config = Configuration.GetConfiguration();

            if (CreatorId != id) return GambleBetResult.UserIsNotAuthor;
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
                return MakeCustomGambleResult(correctOptions);
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
                    var socialUser = config.Users[incorrectUser.DiscordId];
                    var loosedItems = socialUser.DecreaseRating(incorrectUser.Bet).Select(Utility.ItemToString)
                        .ToList();
                    sb.Append($"{socialUser.DiscordUsername}: -{incorrectUser.Bet}");
                    if (loosedItems.Count != 0)
                    {
                        sb.Append($". Проебанные шмотки: {string.Join(", ", loosedItems)}");
                    }

                    sb.Append("\n");
                }

                sb.Append("Все проебали свои пойнты, грац, долбаебы");
                Result = sb.ToString();
                return GambleBetResult.SuccessfulResult;
            }

            var ratio = (double) (totalIncorrectPoints + totalCorrectPoints) / totalCorrectPoints;
            foreach (var correctUser in correctUsers)
            {
                var win = (int) Math.Floor(correctUser.Bet * ratio - correctUser.Bet);
                var socialUser = config.Users[correctUser.DiscordId];
                var newItems = socialUser.IncreaseRating(win).Select(Utility.ItemToString).ToList();
                sb.Append($"{socialUser.DiscordUsername}: +{win}");
                if (newItems.Count != 0)
                {
                    sb.Append($". Полученные шмотки: {string.Join(", ", newItems)}");
                }

                sb.Append("\n");
            }

            foreach (var incorrectUser in incorrectUsers)
            {
                var socialUser = config.Users[incorrectUser.DiscordId];
                var lostItems = socialUser.DecreaseRating(incorrectUser.Bet).Select(Utility.ItemToString).ToList();
                sb.Append($"{socialUser.DiscordUsername}: -{incorrectUser.Bet}");
                if (lostItems.Count != 0)
                {
                    sb.Append($". Проебанные шмотки: {string.Join(", ", lostItems)}");
                }

                sb.Append("\n");
            }

            sb.Append($"Всего проебано {totalIncorrectPoints}");
            Result = sb.ToString();
            return GambleBetResult.SuccessfulResult;
        }

        private GambleBetResult MakeCustomGambleResult(string[] correctStringOptions)
        {
            var config = Configuration.GetConfiguration();

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
                    var win = (int) Math.Floor(correctUser.Bet * correctOption.Ratio - correctUser.Bet);
                    pointsWon += win;
                    var socialUser = config.Users[correctUser.DiscordId];
                    var newItems = socialUser.IncreaseRating(win).Select(Utility.ItemToString).ToList();
                    sb.Append($"{socialUser.DiscordUsername}: +{win}");
                    if (newItems.Count != 0)
                    {
                        sb.Append($". Полученные шмотки: {string.Join(", ", newItems)}");
                    }

                    sb.Append("\n");
                }
            }

            var pointsLost = 0;

            foreach (var incorrectOption in incorrectOptions)
            {
                foreach (var incorrectUser in Bids[incorrectOption])
                {
                    var socialUser = config.Users[incorrectUser.DiscordId];
                    pointsLost += incorrectUser.Bet;
                    var lostItems = socialUser.DecreaseRating(incorrectUser.Bet).Select(Utility.ItemToString).ToList();
                    sb.Append($"{socialUser.DiscordUsername}: -{incorrectUser.Bet}");
                    if (lostItems.Count != 0)
                    {
                        sb.Append($". Проебанные шмотки: {string.Join(", ", lostItems)}");
                    }

                    sb.Append("\n");
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
    }
}