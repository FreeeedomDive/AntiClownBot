using System;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Extensions;
using Hangfire;

namespace AntiClownBotApi.Services
{
    public class TributeService
    {
        private UserRepository UserRepository { get; }
        private GlobalState GlobalState { get; }

        public TributeService(UserRepository userRepository, GlobalState globalState)
        {
            UserRepository = userRepository;
            GlobalState = globalState;
        }

        public TributeResponseDto MakeTribute(ulong userId, bool isAutomatic)
        {
            var response = new TributeResponseDto
            {
                UserId = userId,
                IsTributeAutomatic = isAutomatic
            };

            var user = UserRepository.GetUserWithEconomyAndItems(userId);
            var onlyActiveItems = user.Items.Where(i => i.IsActive).ToList();

            if (!user.IsCooldownPassed())
            {
                response.Result = isAutomatic
                    ? TributeResult.AutoTributeWasCancelledByEarlierTribute
                    : TributeResult.CooldownHasNotPassed;
                return response;
            }

            var cooldownModifiers = UserRepository.UpdateCooldown(onlyActiveItems, userId);

            var minTributeBorder = NumericConstants.MinTributeValue;
            var maxTributeBorder = NumericConstants.MaxTributeValue;
            foreach (var rice in onlyActiveItems.RiceBowls())
            {
                minTributeBorder -= rice.NegativeRangeExtend;
                maxTributeBorder += rice.PositiveRangeExtend;
            }

            var lootBoxChance = onlyActiveItems.DogWives().Select(dog => dog.LootBoxFindChance).Sum();
            if (Randomizer.GetRandomNumberBetween(0, 1000) < lootBoxChance)
            {
                response.HasLootBox = true;
                GlobalState.GiveLootBoxToUser(userId);
            }

            var tributeQuality = Randomizer.GetRandomNumberBetween(minTributeBorder, maxTributeBorder);

            var communismChance = onlyActiveItems
                .CommunismBanners()
                .Select(item => item.DivideChance).Sum();
            var communism = Randomizer.GetRandomNumberBetween(0, 100) < communismChance;

            DbUser sharedUser = null;
            if (communism)
            {
                var sharedUsers = Utility
                    .GetDistributedCommunists(UserRepository)
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

            var isNextTributeAutomatic = Randomizer.GetRandomNumberBetween(0, 100) < onlyActiveItems
                .CatWives()
                .Select(cat => cat.AutoTributeChance)
                .Sum();

            response.IsNextTributeAutomatic = isNextTributeAutomatic;
            response.Result = TributeResult.Success;
            if (!isNextTributeAutomatic) return response;

            SetupAutoTribute(user);

            return response;
        }

        public void SetupAutoTribute(DbUser user)
        {
            var delay = UserRepository.GetUserNextTributeDateTime(user.DiscordId) - DateTime.Now +
                        TimeSpan.FromSeconds(1);
            BackgroundJob.Schedule(() => TributeWithAddToAutoTributesList(user.DiscordId), delay);
        }

        public void TributeWithAddToAutoTributesList(ulong userId)
        {
            var nextResult = MakeTribute(userId, true);
            // TODO send to discord nextResult
            GlobalState.AutomaticTributes.Add(nextResult);
        }
    }
}