using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.SocialRatingCommands
{
    public class RatingCommand: ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            var user = UserDbController.GetUserEconomy(dto.Id);
            return new RatingResponseDto()
            {
                UserId = dto.Id,
                SocialRating = user.Economy.SocialRating
            };
        }

        public string Help()
        {
            return "";
        }
    }
}