using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper.Guilds;
using AntiClown.DiscordBot.DiscordClientWrapper.Members;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Roles;

public class RolesClient : IRolesClient
{
    public RolesClient(
        DiscordClient discordClient,
        IGuildsClient guildsClient,
        IMembersClient membersClient,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.discordClient = discordClient;
        this.guildsClient = guildsClient;
        this.membersClient = membersClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public async Task<DiscordRole> CreateNewRoleAsync(string roleName)
    {
        var guild = await guildsClient.GetGuildAsync();
        var newRole = await guild.CreateRoleAsync(roleName);

        return newRole;
    }

    public async Task<DiscordRole> FindRoleAsync(ulong roleId)
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var guild = discordClient.Guilds[guildId];
        if (!guild.Roles.TryGetValue(roleId, out var role))
        {
            throw new ArgumentException($"Role {roleId} doesn't exist");
        }

        return role;
    }

    public async Task<ulong[]> GetRoleMembersIdsAsync(ulong roleId)
    {
        var members = await membersClient.GetAllAsync();
        return members
               .Where(member => member.Roles.Any(role => role.Id == roleId))
               .Select(x => x.Id)
               .ToArray();
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
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}