using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Data.Api.Core.Rights.Repositories;

[Index(nameof(UserId))]
[Index(nameof(Name))]
public class RightsStorageElement : SqlStorageElement
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
}