using SqlRepositoryBase.Core.Models;

namespace AntiClown.DiscordBot.Interactivity.Repository;

public class InteractivityStorageElement : SqlStorageElement
{
    public string Type { get; set; }
    public ulong MessageId { get; set; }
    public ulong AuthorId { get; set; }
}