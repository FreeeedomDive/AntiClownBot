using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AntiClownBot.Models.BlackJack;
using DSharpPlus;
using DSharpPlus.Entities;
using Emzi0767;
using Newtonsoft.Json;

namespace AntiClownBot
{
    public class Configuration
    {
        public Dictionary<string, int> EmojiStatistics;
        public Dictionary<string, int> PidorStatistics;
        public Dictionary<ulong, SocialRatingUser> Users;

        public DateTime TodayDate;
        public Dictionary<string, int> PidorOfTheDay;
        public ulong CurrentPidorOfTheDay;

        public int PidorRoulette;

        public Gamble CurrentGamble;
        public BlackJack CurrentBlackJack;

        private const string FileName = "config.json";

        private static Configuration GetNewConfiguration()
        {
            return new Configuration
            {
                EmojiStatistics = new Dictionary<string, int>(),
                PidorStatistics = new Dictionary<string, int>(),
                Users = new Dictionary<ulong, SocialRatingUser>(),
                PidorOfTheDay = new Dictionary<string, int>(),
                TodayDate = DateTime.Today,
                CurrentPidorOfTheDay = 0,
                PidorRoulette = Randomizer.GetRandomNumberBetween(5, 40)
            };
        }

        public void CheckCurrentDay()
        {
            var today = DateTime.Today;
            if (TodayDate == today) return;
            TodayDate = today;
            PidorOfTheDay = new Dictionary<string, int>();
            Save();
        }

        public Configuration Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FileName, json);

            return this;
        }

        public static Configuration GetConfiguration()
        {
            var obj = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(FileName));
            return File.Exists(FileName)
                ? obj
                : GetNewConfiguration();
        }

        public string GetEmojiStats(DiscordClient discord)
        {
            return GetStatsForDict(EmojiStatistics, key => DiscordEmoji.FromName(discord, $":{key}:"));
        }

        public string GetPidorStats()
        {
            return GetStatsForDict(PidorStatistics, key => key);
        }

        public string GetSocialRatingStats()
        {
            var dict = Users
                .ToDictionary(
                    pair => pair.Value.DiscordUsername, 
                    pair => pair.Value.SocialRating);
            return GetStatsForDict(dict, key => key);
        }

        private static string GetStatsForDict(Dictionary<string, int> dict, Func<string, string> func)
        {
            var list = dict.ToList();
            list.Sort((pair1, pair2) => -pair1.Value.CompareTo(pair2.Value));
            var sb = new StringBuilder();
            var i = 1;
            foreach (var (key, value) in list)
            {
                try
                {
                    sb.Append($"{i}: {func(key)} - {value}\n");
                }
                catch
                {
                    sb.Append($"{i}: {key} - {value}\n");
                }

                if (i == 25) break;

                i++;
            }

            return sb.ToString();
        }

        public void AddPidor(string username)
        {
            AddRecordToDictionary(PidorStatistics, username);
            CheckCurrentDay();
            AddRecordToDictionary(PidorOfTheDay, username);
        }

        public bool IsPidorOfTheDay(string username)
        {
            var list = PidorOfTheDay.ToList();
            list.Sort((pair1, pair2) => -pair1.Value.CompareTo(pair2.Value));
            if (list.Count < 1) return false;
            return list[0].Key == username;
        }

        public void AddEmoji(string emoji)
        {
            AddRecordToDictionary(EmojiStatistics, emoji);
        }

        private void AddRecordToDictionary(IDictionary<string, int> dict, string key)
        {
            if (dict.ContainsKey(key))
            {
                dict[key]++;
            }
            else
            {
                dict.Add(key, 1);
            }

            Save();
        }

        public void RemoveEmoji(string emoji)
        {
            if (!EmojiStatistics.ContainsKey(emoji)) return;

            var value = EmojiStatistics[emoji];
            if (value == 1)
            {
                EmojiStatistics.Remove(emoji);
            }
            else
            {
                EmojiStatistics[emoji] = value - 1;
            }

            Save();
        }

        public void DecreasePidorRoulette()
        {
            PidorRoulette--;
            Save();
        }

        public bool IsPidor()
        {
            if (PidorRoulette > 0) return false;
            PidorRoulette = Randomizer.GetRandomNumberBetween(5, 40);
            Save();
            return true;
        }
    }
}