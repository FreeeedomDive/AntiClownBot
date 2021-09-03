using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class AllUsersCommand: ICommand
    {
        private UserRepository UserRepository { get; }
        
        public AllUsersCommand(UserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return new AllUsersResponseDto()
            {
                Users = UserRepository.GetAllUsersIds()
            };
        }
    }
}