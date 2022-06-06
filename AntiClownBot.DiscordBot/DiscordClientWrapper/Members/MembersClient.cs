using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Members;

public class MembersClient : IMembersClient
{
    public MembersClient(
        DiscordClient discordClient,
        IGuildSettingsService guildSettingsService,
        ILogger logger
    )
    {
        this.discordClient = discordClient;
        this.guildSettingsService = guildSettingsService;
        this.logger = logger;
    }
    
    public Task<ulong> GetBotIdAsync()
    {
        return Task.FromResult(discordClient.CurrentUser.Id);
    }

    public async Task<DiscordMember> GetAsync(ulong userId)
    {
        var guild = guildSettingsService.GetGuildSettings().GuildId;
        var member = await discordClient.Guilds[guild].GetMemberAsync(userId);

        return member;
    }

    public async Task ModifyAsync(DiscordMember member, Action<MemberEditModel> action)
    {
        await member.ModifyAsync(action);
    }

    private readonly DiscordClient discordClient;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly ILogger logger;

}