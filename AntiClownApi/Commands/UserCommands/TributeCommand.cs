using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.Services;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class TributeCommand : ICommand
    {
        private TributeService TributeService { get; }
        
        public TributeCommand(TributeService tributeService)
        {
            TributeService = tributeService;
        }
        
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return TributeService.MakeTribute(dto.UserId, false);
        }
    }
}