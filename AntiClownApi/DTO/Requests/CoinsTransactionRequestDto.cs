namespace AntiClownBotApi.DTO.Requests
{
    public class CoinsTransactionRequestDto : BaseRequestDto
    {
        public ulong SenderUserId { get; set; }
        public ulong RecipientUserId { get; set; }
        public int Count { get; set; }
    }
}