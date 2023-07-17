namespace AntiClown.DiscordBot.Interactivity.Domain;

public class Interactivity
{
    public Guid Id { get; set; }
    public InteractivityType Type { get; set; }
    public ulong MessageId { get; set; }
    public ulong AuthorId { get; set; }
}