using AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Members;
using Loggers;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Roles;

public class RolesClient : IRolesClient
{
    public RolesClient(
        DiscordClient discordClient,
        IGuildSettingsService guildSettingsService,
        IGuildsClient guildsClient,
        IMembersClient membersClient,
        ILogger logger
    )
    {
        this.discordClient = discordClient;
        this.guildSettingsService = guildSettingsService;
        this.guildsClient = guildsClient;
        this.membersClient = membersClient;
        this.logger = logger;
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

    private readonly DiscordClient discordClient;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IGuildsClient guildsClient;
    private readonly IMembersClient membersClient;
    private readonly ILogger logger;
}