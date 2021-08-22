using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.SocialRatingCommands
{
    public class TributeCommand: ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return MakeTribute(dto.UserId, false);
        }

        private TributeResponseDto MakeTribute(ulong userId, bool isAutomatic)
        {
            var tribute = Randomizer.GetRandomNumberBetween(-50, 100);
            UserDbController.ChangeUserBalance(userId, tribute, "Подношение");

            return new TributeResponseDto()
            {
                UserId = userId,
                TributeQuality = tribute
            };
        }

        public string Help()
        {
            return "Test";
        }
    }
}