namespace AntiClownApiClient.Dto.Responses
{
    public abstract class BaseResponseDto
    {
        public ulong UserId { get; set; }
        public bool HasError { get; set; } = false;
        public string Error { get; set; } = "";
    }
}