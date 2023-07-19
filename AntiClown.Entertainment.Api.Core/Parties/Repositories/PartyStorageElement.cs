using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.Parties.Repositories;

public class PartyStorageElement : VersionedSqlStorageElement
{
    public string Name { get; set; }
    public bool IsOpened { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SerializedParty { get; set; }
}