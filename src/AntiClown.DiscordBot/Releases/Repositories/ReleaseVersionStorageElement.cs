using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.DiscordBot.Releases.Repositories;

[Index(nameof(CreatedAt))]
public class ReleaseVersionStorageElement : SqlStorageElement
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}