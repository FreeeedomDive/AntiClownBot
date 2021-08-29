using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class ChangeUserBalanceCommand: ICommand
    {
        private GlobalState GlobalState { get; }

        public ChangeUserBalanceCommand(GlobalState globalState)
        {
            GlobalState = globalState;
        }
        
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            if (dto is not ChangeUserBalanceRequestDto request)
            {
                return new ChangeUserBalanceResponseDto()
                {
                    UserId = dto.UserId,
                    ChangeUserRatingResult = ChangeUserRatingResult.Error
                };
            }
            GlobalState.ChangeUserBalance(request.UserId, request.RatingDiff, request.Reason);
            return new ChangeUserBalanceResponseDto()
            {
                UserId = dto.UserId,
                ChangeUserRatingResult = ChangeUserRatingResult.Success
            };
        }
    }
}