namespace AntiClownBotApi.DTO.Requests
{
    public class ChangeUserBalanceRequestDto: BaseRequestDto
    {
        public int RatingDiff { get; set; }
        public string Reason { get; set; }
    }
}