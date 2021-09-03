using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class RemoveCooldownsCommand : ICommand
    {
        private UserRepository UserRepository { get; }
        
        public RemoveCooldownsCommand(UserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            UserRepository.RemoveCooldowns();
            return new BaseResponseDto();
        }
    }
}