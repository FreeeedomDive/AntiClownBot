using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AntiClownBot.Helpers;
using AntiClownBot.Models.BlackJack;
using AntiClownBot.Models.Lottery;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Roulette;
using AntiClownBot.Models.DailyStatistics;
using AntiClownBot.Models.Gamble;
using AntiClownBot.Models.Gaming;
using AntiClownBot.Models.GuessNumber;
using AntiClownBot.Models.Inventory;
using AntiClownBot.Models.Lohotron;
using AntiClownBot.Models.Race;
using AntiClownBot.Models.Shop;
using DSharpPlus.EventArgs;

namespace AntiClownBot
{
    public class Configuration
    {
        public const bool IsMaintenanceMode = false;
        
        public Dictionary<string, int> EmojiStatistics;
        public DateTime TodayDate;
        public DailyStatistics DailyStatistics;

        [JsonIgnore] public RouletteGame Roulette = new RouletteGame();

        public Lohotron DailyScamMachine;
        public Gamble CurrentGamble;
        [JsonIgnore] public BlackJack CurrentBlackJack = new BlackJack();
        public Lottery CurrentLottery;
        [JsonIgnore] public RaceModel CurrentRace;
        [JsonIgnore] public GuessNumberGame CurrentGuessNumberGame;
        [JsonIgnore] public Dictionary<ulong, GameParty> OpenParties = new();
        [JsonIgnore] private DiscordMessage _partyObserver;
        [JsonIgnore] public Dictionary<ulong, Shop> Shops = new();
        [JsonIgnore] public Dictionary<ulong, UserInventory> Inventories = new();

        private const string FileName = "config.json";

        private static Configuration instance;

        private static Configuration GetNewConfiguration()
        {
            return new Configuration
            {
                EmojiStatistics = new Dictionary<string, int>(),
                TodayDate = DateTime.Today,
                DailyStatistics = new DailyStatistics()
            };
        }

        public void CheckCurrentDay()
        {
            var today = DateTime.Today;
            if (TodayDate == today) return;
            TodayDate = today;
            DailyStatistics = new DailyStatistics();
            DailyScamMachine = new Lohotron();
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
            if (instance != null) return instance;
            if (!File.Exists(FileName))
            {
                instance = GetNewConfiguration();
                instance.Save();
                return instance;
            }

            instance = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(FileName));

            instance.DailyStatistics ??= new DailyStatistics();
            instance.DailyScamMachine ??= new Lohotron();
            instance.Save();
            return instance;
        }
        
        public void ChangeBalance(ulong userId, int rating, string reason)
        {
            ApiWrapper.Wrappers.UsersApi.ChangeUserRating(userId, rating, reason);
            DailyStatistics.CreditsCollected += rating;
            DailyStatistics.ChangeUserCredits(userId, rating);
            Configuration.GetConfiguration().Save();
        }
        
        public void BulkChangeBalance(ulong userId, int rating, string reason){
            ApiWrapper.Wrappers.UsersApi.ChangeUserRating(userId, rating, reason);
            DailyStatistics.CreditsCollected += rating;
            DailyStatistics.ChangeUserCredits(userId, rating);
            Configuration.GetConfiguration().Save();
        }

        public static DiscordMember GetServerMember(ulong userId) =>
            Utility.Client.Guilds[Constants.GuildId].GetMemberAsync(userId).Result;

        public static int GetUserBalance(ulong userId)
        {
            var result = ApiWrapper.Wrappers.UsersApi.Rating(userId);
            return result.ScamCoins;
        }

        public string GetEmojiStats()
        {
            return GetStatsForDict(EmojiStatistics, key => Utility.Emoji($":{key}:"));
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

        private DiscordEmbed GetPartiesEmbed()
        {
            var embedBuilder = new DiscordEmbedBuilder();
            var partiesLinks = 
                OpenParties
                .Values
                .ToDictionary(p => p,
                    p =>
                        @$"https://discord.com/channels/{Constants.GuildId}/{p.Message.ChannelId}/{p.Message.Id}");

            embedBuilder.WithTitle("ТЕКУЩИЕ ПАТИ");
            if (OpenParties.Count == 0)
            {
                embedBuilder.Color = new Optional<DiscordColor>(DiscordColor.DarkRed);
                embedBuilder.AddField($"{Utility.Emoji(":BibleThump:")}", "Сейчас никто не играет");
            }
            else
            {
                embedBuilder.Color = new Optional<DiscordColor>(DiscordColor.DarkGreen);
                // ToList нужен для срабатывания ленивого foreach, так как без вызова неленивого метода коллекция не будет пройдена в цикле
                _ = partiesLinks.ForEach(
                    (kv) => embedBuilder.AddField(
                        $"{kv.Key.Description} - {kv.Key.Players.Count} / {kv.Key.MaxPlayersCount} игроков",
                        @$"[Ссылка]({kv.Value})")).ToList();
            }

            return embedBuilder.Build();
        }
        
        public async void AddPartyObserverMessage(MessageCreateEventArgs e)
        {
            if (_partyObserver != null)
                await _partyObserver.DeleteAsync();
            var message = await e.Message.RespondAsync(GetPartiesEmbed());
            _partyObserver = message;
        }

        public void DeleteObserverIfExists(DiscordMessage message)
        {
            if (_partyObserver == null) return;
            if (_partyObserver.Id != message.Id) return;
            _partyObserver = null;
        }

        public async void UpdatePartyObservers()
        {
            if (_partyObserver == null) return;
            await _partyObserver.ModifyAsync(GetPartiesEmbed());
        }
    }
}