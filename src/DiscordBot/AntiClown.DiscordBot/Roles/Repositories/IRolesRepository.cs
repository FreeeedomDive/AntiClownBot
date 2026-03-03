namespace AntiClown.DiscordBot.Roles.Repositories;

public interface IRolesRepository
{
    Task<ulong[]> ReadAllAsync();
    Task<bool> ExistsAsync(ulong roleId);
    Task CreateAsync(ulong discordRoleId);
}