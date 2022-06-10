namespace ApiWrapper.Responses.UserCommandResponses
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