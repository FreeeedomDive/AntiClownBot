using AntiClownApiClient.Dto.Responses;

namespace AntiClownApiClient.Dto.Responses.UserCommandResponses
{
    public class ChangeUserBalanceResponseDto: BaseResponseDto
    {
        public Result Result { get; set; }
    }

    public enum Result
    {
        Success,
        Error
    }
}