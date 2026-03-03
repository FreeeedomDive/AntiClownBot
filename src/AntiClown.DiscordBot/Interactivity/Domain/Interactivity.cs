namespace AntiClown.DiscordBot.Interactivity.Domain;

public class Interactivity<T>
{
    public Guid Id { get; set; }
    public InteractivityType Type { get; set; }
    public ulong MessageId { get; set; }
    public ulong AuthorId { get; set; }
    public T? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}