using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class AllUsersCommand: ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return new AllUsersResponseDto()
            {
                Users = UserDbController.GetAllUsersIds()
            };
        }
    }
}