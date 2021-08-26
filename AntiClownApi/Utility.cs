using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Models.Items;
using Newtonsoft.Json;

namespace AntiClownBotApi
{
    public static class Utility
    {
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
        
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> function)
        {
            foreach (var item in items)
            {
                function(item);
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
                sb.Append(PluralizeString(diff.Hours, "час", "часа", "часов")).Append(' ');
            if (diff.Minutes != 0)
                sb.Append(PluralizeString(diff.Minutes, "минуту", "минуты", "минут")).Append(' ');
            if (diff.Seconds != 0)
                sb.Append(PluralizeString(diff.Seconds, "секунду", "секунды", "секунд"));

            return sb.ToString();
        }

        private static string PluralizeString(int count, string singleForm, string severalForm, string manyForm)
        {
            var correctCount = count % 100;
            if (correctCount is >= 10 and <= 20 || correctCount % 10 >= 5 && correctCount % 10 <= 9 ||
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
        
        public static readonly Dictionary<Rarity, int> Prices = new()
        {
            {Rarity.Common, 1000},
            {Rarity.Rare, 2500},
            {Rarity.Epic, 4500},
            {Rarity.Legendary, 10000},
            {Rarity.BlackMarket, 20000}
        };

        public static Rarity GenerateRarity() => Randomizer.GetRandomNumberBetween(0, 1000000) switch
        {
            >= 0 and <= 666666 => Rarity.Common,
            > 666666 and <= 930228 => Rarity.Rare,
            > 930228 and <= 999000 => Rarity.Epic,
            > 999000 and <= 999998 => Rarity.Legendary,
            _ => Rarity.BlackMarket
        };
        
        public static List<DbUser> GetCommunists() =>
            UserDbController.GetAllUsersWithEconomyAndItems()
                .Where(user => user.Items.Any(item => item.Name.Equals(StringConstants.CommunismBannerName)))
                .ToList();

        public static Dictionary<DbUser, int> GetCommunistsDictionary() =>
            GetCommunists()
                .ToDictionary(
                    user => user,
                    user => user.Items.Count(item => item.Name.Equals(StringConstants.CommunismBannerName))
                );

        public static List<DbUser> GetDistributedCommunists()
        {
            var result = new List<DbUser>();
            foreach (var (user, count) in GetCommunistsDictionary())
            {
                for (var i = 0; i < count; i++)
                {
                    result.Add(user);
                }
            }

            return result;
        }

        public static string GetPosgreSqlConfigureStringFromFile()
        {
            var jsonText = File.ReadAllText("bdConfig.json");
            return JsonConvert.DeserializeObject<DbConfigureModel>(jsonText)?.Configuration;
        }
    }
}