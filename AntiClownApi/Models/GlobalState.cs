using System;
using System.Collections.Generic;
using AntiClownBotApi.Database.DBControllers;
using Newtonsoft.Json;

namespace AntiClownBotApi.Models
{
    public static class GlobalState
    {
        public static DateTime TodayDate;
        // public static RouletteGame Roulette = new();
        // public static Lohotron DailyScamMachine;
        // public static Gamble CurrentGamble;
        // public static BlackJack CurrentBlackJack = new();
        // public static Lottery CurrentLottery;
        // public static RaceModel CurrentRace;
        // public static GuessNumberGame CurrentGuessNumberGame;
        // public static Dictionary<ulong, GameParty> OpenParties = new();

        // public void CheckCurrentDay()
        // {
        //     var today = DateTime.Today;
        //     if (TodayDate == today) return;
        //     TodayDate = today;
        //     DailyScamMachine = new Lohotron();
        // }

        // public string GetSocialRatingStats()
        // {
        //     var dict = Users
        //         .ToDictionary(
        //             pair => pair.Value.DiscordUsername,
        //             pair => pair.Value.NetWorth);
        //     return GetStatsForDict(dict, key => key);
        // }

        public static void ChangeUserBalance(ulong userId, int ratingDiff, string reason)
        {
            UserDbController.ChangeUserBalance(userId, ratingDiff, reason);
        }

        public static int GetUserBalance(ulong userId) => UserDbController.GetUserEconomy(userId).Economy.ScamCoins;

        // TODO: To Database

        private static bool AreTributesOpen = true;

        public static void CloseTributes()
        {
            AreTributesOpen = false;
        }

        public static void OpenTributes()
        {
            AreTributesOpen = true;
        }
    }
}