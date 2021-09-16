namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class CoinsTransactionResponseDto : BaseResponseDto
    {
        public CoinsTransactionResult Result { get; set; }
    }
    
    public enum CoinsTransactionResult
    {
        Success,
        Error
    }
}