using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class WhenCommand: ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return new WhenNextTributeResponseDto()
            {
                UserId = dto.UserId,
                NextTribute = UserDbController.GetUserNextTributeDateTime(dto.UserId)
            };
        }
    }
}