using System;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models.Items;
using Hangfire;

namespace AntiClownBotApi.Services
{
    public class TributeService
    {
        public static TributeResponseDto MakeTribute(ulong userId, bool isAutomatic)
        {
            var response = new TributeResponseDto()
            {
                UserId = userId,
                IsTributeAutomatic = isAutomatic
            };

            var user = UserDbController.GetUserWithEconomyAndItems(userId);

            if (!user.IsCooldownPassed())
            {
                response.Result = isAutomatic
                    ? TributeResult.AutoTributeWasCancelledByEarlierTribute
                    : TributeResult.CooldownHasNotPassed;
                return response;
            }

            var cooldownModifiers = user.UpdateCooldown();

            var minTributeBorder = NumericConstants.MinTributeValue;
            var maxTributeBorder = NumericConstants.MaxTributeValue;
            foreach (var rice in user.Items.Where(item => item.Name.Equals(StringConstants.RiceBowlName))
                .Select(item => (RiceBowl) item))
            {
                minTributeBorder -= rice.NegativeRangeExtend;
                maxTributeBorder += rice.PositiveRangeExtend;
            }

            var tributeQuality = Randomizer.GetRandomNumberBetween(minTributeBorder, maxTributeBorder);

            var communismChance = user.Items
                .Where(item => item.Name.Equals(StringConstants.CommunismBannerName))
                .Select(item => (CommunismBanner) item)
                .Select(item => item.DivideChance).Sum();
            var communism = Randomizer.GetRandomNumberBetween(0, 100) < communismChance;

            DbUser sharedUser = null;
            if (communism)
            {
                var sharedUsers = Utility
                    .GetDistributedCommunists()
                    .Where(u => u.DiscordId != user.DiscordId)
                    .ToList();
                if (sharedUsers.Count > 0)
                {
                    sharedUser = sharedUsers.SelectRandomItem();
                    tributeQuality /= 2;
                }
            }

            response.TributeQuality = tributeQuality;
            response.CooldownModifiers = cooldownModifiers;

            GlobalState.ChangeUserBalance(userId, tributeQuality, "Подношение");
            if (communism && sharedUser is not null)
            {
                response.IsCommunismActive = true;
                response.SharedCommunistUserId = sharedUser.DiscordId;
                GlobalState.ChangeUserBalance(sharedUser.DiscordId, tributeQuality,
                    "Полученное разделенное подношение");
            }

            var isNextTributeAutomatic = Randomizer.GetRandomNumberBetween(0, 100) < user.Items
                .Where(item => item.Name.Equals(StringConstants.CatWifeName))
                .Select(item => (CatWife) item)
                .Select(cat => cat.AutoTributeChance)
                .Sum();

            response.IsNextTributeAutomatic = isNextTributeAutomatic;
            response.Result = TributeResult.Success;
            if (!isNextTributeAutomatic) return response;

            SetupAutoTribute(user);

            return response;
        }

        public static void SetupAutoTribute(DbUser user)
        {
            var delay = UserDbController.GetUserNextTributeDateTime(user.DiscordId) - DateTime.Now +
                        TimeSpan.FromSeconds(1);
            BackgroundJob.Schedule(() => TributeWithAddToAutoTributesList(user.DiscordId), delay);
        }
        
        public static void TributeWithAddToAutoTributesList(ulong userId)
        {
            var nextResult = MakeTribute(userId, true);
            // TODO send to discord nextResult
            GlobalState.AutomaticTributes.Add(nextResult);
        }
    }
}