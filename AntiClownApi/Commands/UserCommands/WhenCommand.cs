using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class WhenCommand: ICommand
    {
        private UserRepository UserRepository { get; }

        public WhenCommand(UserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            return new WhenNextTributeResponseDto()
            {
                UserId = dto.UserId,
                NextTribute = UserRepository.GetUserNextTributeDateTime(dto.UserId)
            };
        }
    }
}