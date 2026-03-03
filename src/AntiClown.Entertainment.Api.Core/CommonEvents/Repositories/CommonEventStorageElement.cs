using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;

public class CommonEventStorageElement : SqlStorageElement
{
    public string Type { get; set; }
    public DateTime EventDateTime { get; set; }
    public bool Finished { get; set; }
    public string Details { get; set; }
}