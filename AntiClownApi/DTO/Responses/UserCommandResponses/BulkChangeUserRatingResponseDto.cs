namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class BulkChangeUserRatingResponseDto: BaseResponseDto
    {
        public BulkChangeUserRatingResult BulkChangeUserRatingResult { get; set; }
    }

    public enum BulkChangeUserRatingResult
    {
        Success,
        Error
    }
}