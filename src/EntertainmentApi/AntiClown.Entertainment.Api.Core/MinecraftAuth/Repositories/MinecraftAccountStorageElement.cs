using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;

[Index(nameof(Username))]
[Index(nameof(DiscordId))]
public class MinecraftAccountStorageElement : SqlStorageElement
{
    // Id это UserId

    public string Username { get; set; }

    public string? UsernameAndPasswordHash { get; set; }

    public string? AccessTokenHash { get; set; }

    public string? SkinUrl { get; set; }

    public string? CapeUrl { get; set; }

    public string DiscordId { get; set; }
}