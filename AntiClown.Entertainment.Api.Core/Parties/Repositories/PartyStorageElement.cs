using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.Parties.Repositories;

[Index(nameof(CreatorId))]
[Index(nameof(FirstFullPartyAt))]
public class PartyStorageElement : VersionedSqlStorageElement
{
    public string Name { get; set; }
    public bool IsOpened { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime? FirstFullPartyAt { get; set; }
    public string SerializedParty { get; set; }
}