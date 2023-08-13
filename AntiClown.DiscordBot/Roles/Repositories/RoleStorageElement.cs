using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.DiscordBot.Roles.Repositories;

[Index(nameof(DiscordRoleId))]
public class RoleStorageElement : SqlStorageElement
{
    public ulong DiscordRoleId { get; set; }
}