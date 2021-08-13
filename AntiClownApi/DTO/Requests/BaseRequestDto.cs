namespace AntiClownBotApi.DTO.Requests
{
    public class BaseRequestDto
    {
        public ulong Id { get; set; }

        public BaseRequestDto(ulong id)
        {
            Id = id;
        }
    }
}