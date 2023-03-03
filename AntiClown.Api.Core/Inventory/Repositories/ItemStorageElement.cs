using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Inventory.Repositories;

[Index(nameof(OwnerId), nameof(IsActive), nameof(Name))]
public class ItemStorageElement : SqlStorageElement
{
    public Guid OwnerId { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string ItemSpecs { get; set; }
}