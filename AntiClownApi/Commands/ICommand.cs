using AntiClownBotApi.DTO;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;

namespace AntiClownBotApi.Commands
{
    public interface ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto);
        public string Help();
    }
}