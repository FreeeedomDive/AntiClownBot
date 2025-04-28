namespace AntiClown.Api.Core.Users.Domain;

public class User
{
    public Guid Id { get; set; }
    public ulong DiscordId { get; set; }
    public long? TelegramId { get; set; }
    public string? YandexId { get; set; }
}