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
            var tribute = Randomizer.GetRandomNumberBetween(-50, 100);
            UserDbController.ChangeUserRating(dto.Id, tribute, "test");
            
            return new TributeResponseDto()
            {
                UserId = dto.Id,
                TributeQuality = tribute
            };
        }

        public string Help()
        {
            return "Test";
        }
    }
}