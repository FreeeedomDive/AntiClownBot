using SqlRepositoryBase.Core.Models;

namespace AntiClown.DiscordBot.Utility.Locks;

public class LockStorageElement : SqlStorageElement
{
    public string Key { get; set; }
}