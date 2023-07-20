using AntiClown.DiscordBot.DiscordClientWrapper.Guilds;
using AntiClown.DiscordBot.DiscordClientWrapper.Members;
using AntiClown.DiscordBot.Options;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Roles;

public class RolesClient : IRolesClient
{
    public RolesClient(
        DiscordClient discordClient,
        IGuildsClient guildsClient,
        IMembersClient membersClient,
        IOptions<DiscordOptions> discordOptions
    )
    {
        this.discordClient = discordClient;
        this.guildsClient = guildsClient;
        this.membersClient = membersClient;
        this.discordOptions = discordOptions;
    }

    public async Task<DiscordRole> CreateNewRoleAsync(string roleName)
    {
        var guild = await guildsClient.GetGuildAsync();
        var newRole = await guild.CreateRoleAsync(roleName);

        return newRole;
    }

    public Task<DiscordRole> FindRoleAsync(ulong roleId)
    {
        var guild = discordClient.Guilds[discordOptions.Value.GuildId];
        if (!guild.Roles.ContainsKey(roleId))
        {
            throw new ArgumentException($"Role {roleId} doesn't exist");
        }

        return Task.FromResult(guild.Roles[roleId]);
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

    private readonly DiscordClient discordClient;
    private readonly IGuildsClient guildsClient;
    private readonly IMembersClient membersClient;
    private readonly IOptions<DiscordOptions> discordOptions;
}