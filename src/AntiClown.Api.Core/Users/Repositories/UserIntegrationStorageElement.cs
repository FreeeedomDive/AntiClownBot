using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Users.Repositories;

[Index(nameof(IntegrationName), nameof(IntegrationUserId))]
public class UserIntegrationStorageElement : SqlStorageElement
{
    [Column]
    public Guid UserId { get; set; }
    
    [Column]
    public string IntegrationName { get; set; }

    [Column]
    public string IntegrationUserId { get; set; }
}