using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Members;

public class MembersClient : IMembersClient
{
    public MembersClient(
        DiscordClient discordClient,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.discordClient = discordClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public Task<ulong> GetBotIdAsync()
    {
        return Task.FromResult(discordClient.CurrentUser.Id);
    }

    public async Task<DiscordMember[]> GetAllAsync()
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        return (await discordClient.Guilds[guildId].GetAllMembersAsync()).ToArray();
    }

    public async Task<DiscordMember> GetAsync(ulong userId)
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var member = await discordClient.Guilds[guildId].GetMemberAsync(userId);

        return member;
    }

    public async Task ModifyAsync(DiscordMember member, Action<MemberEditModel> action)
    {
        await member.ModifyAsync(action);
    }

    private readonly DiscordClient discordClient;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}