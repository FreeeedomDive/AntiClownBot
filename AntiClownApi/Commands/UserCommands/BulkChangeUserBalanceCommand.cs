using AntiClownBotApi.DTO.Requests;
using AntiClownBotApi.DTO.Responses;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;

namespace AntiClownBotApi.Commands.UserCommands
{
    public class BulkChangeUserBalanceCommand : ICommand
    {
        public BaseResponseDto Execute(BaseRequestDto dto)
        {
            if (dto is not BulkChangeUserBalanceRequestDto request)
            {
                return new BulkChangeUserRatingResponseDto()
                {
                    UserId = dto.UserId,
                    BulkChangeUserRatingResult = BulkChangeUserRatingResult.Error
                };
            }
            
            request.Users.ForEach(user => GlobalState.ChangeUserBalance(user, request.RatingDiff, request.Reason));
            
            return new ChangeUserBalanceResponseDto()
            {
                UserId = dto.UserId,
                ChangeUserRatingResult = ChangeUserRatingResult.Success
            };
        }
    }
}