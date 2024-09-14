namespace AntiClown.Api.Dto.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public ulong DiscordId { get; set; }
    public long? TelegramId { get; set; }
}