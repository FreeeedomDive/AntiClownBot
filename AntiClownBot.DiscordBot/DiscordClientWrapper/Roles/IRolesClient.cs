using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Roles;

public interface IRolesClient
{
    Task<DiscordRole> CreateNewRoleAsync(string roleName);
    Task<DiscordRole> FindRoleAsync(ulong roleId);
    Task GrantRoleAsync(ulong userId, DiscordRole role);
    Task RevokeRoleAsync(ulong userId, DiscordRole role);
}