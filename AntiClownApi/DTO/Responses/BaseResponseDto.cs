namespace AntiClownBotApi.DTO.Responses
{
    public class BaseResponseDto
    {
        public ulong UserId { get; set; }
        public bool HasError { get; set; } = false;
        public string Error { get; set; } = "";
    }
}