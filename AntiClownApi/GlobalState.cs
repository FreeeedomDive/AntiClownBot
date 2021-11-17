using System;
using System.Collections.Generic;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi
{
    public class GlobalState
    {
        public static List<TributeResponseDto> AutomaticTributes = new();

        private UserRepository UserRepository { get; }
        // public static DateTime TodayDate;
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

        public GlobalState(UserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public void ChangeUserBalance(ulong userId, int ratingDiff, string reason) =>
            UserRepository.ChangeUserBalance(userId, ratingDiff, reason);

        public void GiveLootBoxToUser(ulong userId) => UserRepository.GiveLootBoxToUser(userId);
        public void RemoveLootBoxFromUser(ulong userId) => UserRepository.RemoveLootBoxFromUser(userId);

        public int GetUserBalance(ulong userId) => UserRepository.GetUserEconomy(userId).Economy.ScamCoins;
    }
}