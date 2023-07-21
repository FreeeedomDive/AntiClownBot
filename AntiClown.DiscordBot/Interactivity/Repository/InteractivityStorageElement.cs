using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.DiscordBot.Interactivity.Repository;

[Index(nameof(Type))]
public class InteractivityStorageElement : SqlStorageElement
{
    public string Type { get; set; }
    public ulong MessageId { get; set; }
    public ulong AuthorId { get; set; }
    public string Details { get; set; }
    public DateTime CreatedAt { get; set; }
}