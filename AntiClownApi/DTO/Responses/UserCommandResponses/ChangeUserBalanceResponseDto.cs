namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class ChangeUserBalanceResponseDto: BaseResponseDto
    {
        public ChangeUserRatingResult ChangeUserRatingResult { get; set; }
    }

    public enum ChangeUserRatingResult
    {
        Success,
        Error
    }
}