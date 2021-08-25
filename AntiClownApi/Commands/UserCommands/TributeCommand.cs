using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class TributeCommand : ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return MakeTribute(dto.UserId, false);
        }

        private TributeResponseDto MakeTribute(ulong userId, bool isAutomatic)
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

        private void SetupAutoTribute(DbUser user)
        {
            var thread = new Thread(async () =>
            {
                var delay = (UserDbController.GetUserNextTributeDateTime(user.DiscordId) - DateTime.Now).TotalMilliseconds + 1000;
                await Task.Delay((int) delay);
                var nextResult = MakeTribute(user.DiscordId, true);
                // TODO send to discord nextResult
                GlobalState.AutomaticTributes.Add(nextResult);
            })
            {
                IsBackground = true
            };
            thread.Start();
        }
    }
}