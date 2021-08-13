namespace AntiClownBotApi.DTO.Requests
{
    public class UserDto: BaseRequestDto
    {
        public string Name { get; init; }

        public UserDto(ulong id) : base(id)
        {
        }
    }
}