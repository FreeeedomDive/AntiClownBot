using AntiClown.DiscordBot.DiscordClientWrapper.Guilds;
using AntiClown.DiscordBot.DiscordClientWrapper.Members;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Roles;

public class RolesClient : IRolesClient
{
    public RolesClient(
        IGuildsClient guildsClient,
        IMembersClient membersClient
    )
    {
        this.guildsClient = guildsClient;
        this.membersClient = membersClient;
    }

    public async Task<DiscordRole> CreateNewRoleAsync(string roleName)
    {
        var guild = await guildsClient.GetGuildAsync();
        var newRole = await guild.CreateRoleAsync(roleName);

        return newRole;
    }

    public async Task<DiscordRole> FindRoleAsync(ulong roleId)
    {
        var guild = await guildsClient.GetGuildAsync();
        if (!guild.Roles.ContainsKey(roleId))
        {
            throw new ArgumentException($"Role {roleId} doesn't exist");
        }

        return guild.Roles[roleId];
    }

    public async Task GrantRoleAsync(ulong userId, DiscordRole role)
    {
        var member = await membersClient.GetAsync(userId);
        await member.GrantRoleAsync(role);
    }

    public async Task RevokeRoleAsync(ulong userId, DiscordRole role)
    {
        var member = await membersClient.GetAsync(userId);
        await member.RevokeRoleAsync(role);
    }

    private readonly IGuildsClient guildsClient;
    private readonly IMembersClient membersClient;
}