using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntiClownBot.Models.BlackJack;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot
{
    public static class Utility
    {
        public static DiscordClient Client;

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) => items.OrderBy(_ => Randomizer.GetRandomNumberBetween(0, 1000000));

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

        public static string Repeat(this string s, int count) => string.Concat(Enumerable.Repeat(s, count));

        public static string KeyValuePairToString<T1,T2>(KeyValuePair<T1,T2> pair)
        {
            return $"{pair.Key} : {pair.Value}";
        }
        public static string ItemToString(InventoryItem item)
        {
            return item switch
            {
                InventoryItem.CatWife => "кошка-жена",
                InventoryItem.DogWife => "собака-жена",
                InventoryItem.RiceBowl => "рис миска",
                InventoryItem.Gigabyte => "гигабайт интернет",
                InventoryItem.JadeRod => "нефритовый стержень",
                InventoryItem.CommunismPoster => "коммунистический плакат",
                InventoryItem.None => "",
                _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
            };
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
    }
}