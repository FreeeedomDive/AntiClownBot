namespace ApiWrapper.Responses.UserCommandResponses
{
    public class BulkChangeUserBalanceResponseDto: BaseResponseDto
    {
        public BulkChangeUserBalanceResult BulkChangeUserBalanceResult { get; set; }
    }

    public enum BulkChangeUserBalanceResult
    {
        Success,
        Error
    }
}