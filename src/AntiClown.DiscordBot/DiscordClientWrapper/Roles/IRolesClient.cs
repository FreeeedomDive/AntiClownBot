using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Roles;

public interface IRolesClient
{
    Task<DiscordRole> CreateNewRoleAsync(string roleName);
    Task<DiscordRole> FindRoleAsync(ulong roleId);
    Task<ulong[]> GetRoleMembersIdsAsync(ulong roleId);
    Task GrantRoleAsync(ulong userId, DiscordRole role);
    Task RevokeRoleAsync(ulong userId, DiscordRole role);
}