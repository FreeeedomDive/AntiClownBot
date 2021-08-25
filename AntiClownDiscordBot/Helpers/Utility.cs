using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiClownBot.Models.BlackJack;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownBot.Helpers
{
    public static class Utility
    {
        public static DiscordClient Client;

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) =>
            items.OrderBy(_ => Randomizer.GetRandomNumberBetween(0, 1000000));

        public static T SelectRandomItem<T>(this IEnumerable<T> items)
        {
            var list = items.ToList();
            return list[Randomizer.GetRandomNumberBetween(0, list.Count)];
        }

        public static Queue<T> WithoutItem<T>(this Queue<T> queue, T removableItem)
        {
            var newQueue = new Queue<T>();
            foreach (var item in queue.Where(item => !item.Equals(removableItem)))
            {
                newQueue.Enqueue(item);
            }

            return newQueue;
        }
        
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> function)
        {
            foreach (var item in items)
            {
                function(item);
                yield return item;
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T, int> function)
        {
            var i = 0;
            foreach (var item in items)
            {
                function(item, i++);
                yield return item;
            }
        }

        public static string Repeat(this string s, int count) => string.Concat(Enumerable.Repeat(s, count));

        public static string KeyValuePairToString<T1, T2>(KeyValuePair<T1, T2> pair)
        {
            var (key, value) = pair;
            return $"{key} : {value}";
        }

        public static int CardToInt(Card card)
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

        public static string StringEmoji(string emoji) => $"{DiscordEmoji.FromName(Client, emoji)}";
        public static DiscordEmoji Emoji(string emoji) => DiscordEmoji.FromName(Client, emoji);

        public static string NormalizeTime(DateTime dateTime)
        {
            return $"{Normalize(dateTime.Hour)}:{Normalize(dateTime.Minute)}:{Normalize(dateTime.Second)}";
        }

        public static string NormalizeTime(int totalTime)
        {
            var ms = AddLeadingZeros(3, totalTime % 1000);
            totalTime /= 1000;
            var sec = AddLeadingZeros(2, totalTime % 60);
            totalTime /= 60;
            return $"{totalTime}:{sec}.{ms}";
        }

        public static string AddLeadingZeros(int totalNumbers, int time)
        {
            var leadingZerosCount = totalNumbers - time.ToString().Length;
            return $"{"0".Repeat(leadingZerosCount)}{time}";
        }

        private static string Normalize(int number)
        {
            return number < 10 ? $"0{number}" : $"{number}";
        }

        public static string GetTimeDiff(DateTime dateTime)
        {
            var diff = dateTime - DateTime.Now;
            var sb = new StringBuilder();
            if (diff.Hours != 0)
                sb.Append(PluralizeString(diff.Hours, "час", "часа", "часов")).Append(" ");
            if (diff.Minutes != 0)
                sb.Append(PluralizeString(diff.Minutes, "минуту", "минуты", "минут")).Append(" ");
            if (diff.Seconds != 0)
                sb.Append(PluralizeString(diff.Seconds, "секунду", "секунды", "секунд"));

            return sb.ToString();
        }

        private static string PluralizeString(int count, string singleForm, string severalForm, string manyForm)
        {
            var correctCount = count % 100;
            if (correctCount >= 10 && correctCount <= 20 || correctCount % 10 >= 5 && correctCount % 10 <= 9 ||
                correctCount % 10 == 0)
                return $"{count} {manyForm}";
            return correctCount % 10 == 1 ? $"{count} {singleForm}" : $"{count} {severalForm}";
        }

        public static int LogarithmicDistribution(int startValue, int count)
        {
            if (count == 0) return 0;
            var result = startValue;
            var ratio = startValue / 2;
            for (var i = 0; i < count - 1; i++)
            {
                result += Math.Max(1, ratio);
                ratio /= 2;
            }

            return result;
        }

        public static async Task<DiscordMessage> SendMessageToBotChannel(string content)
        {
            return await Client
                .Guilds[Constants.GuildId]
                .GetChannel(Constants.BotChannelId)
                .SendMessageAsync(content);
        }

        public static string ServerOrUsername(this DiscordMember member)
        {
            return member.Nickname ?? member.Username;
        }
    }
}