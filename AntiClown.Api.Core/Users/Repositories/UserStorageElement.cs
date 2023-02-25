using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Users.Repositories;

public class UserStorageElement : SqlStorageElement
{
    public ulong DiscordId { get; set; }
}