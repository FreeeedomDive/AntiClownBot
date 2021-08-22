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
            var user = UserDbController.GetUserEconomy(dto.UserId);
            return new RatingResponseDto()
            {
                UserId = dto.UserId,
                SocialRating = user.Economy.ScamCoins
            };
        }

        public string Help()
        {
            return "";
        }
    }
}