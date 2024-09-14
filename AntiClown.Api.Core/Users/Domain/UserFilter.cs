namespace AntiClown.Api.Core.Users.Domain;

public class UserFilter
{
    public ulong? DiscordId { get; set; }
    public long? TelegramId { get; set; }
}