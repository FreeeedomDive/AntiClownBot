namespace AntiClown.Api.Dto.Users;

public class UserFilterDto
{
    public ulong? DiscordId { get; set; }
    public long? TelegramId { get; set; }
}