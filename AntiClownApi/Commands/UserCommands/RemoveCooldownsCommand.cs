using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class RemoveCooldownsCommand: ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            UserDbController.RemoveCooldowns();
            return new BaseResponseDto();
        }
    }
}