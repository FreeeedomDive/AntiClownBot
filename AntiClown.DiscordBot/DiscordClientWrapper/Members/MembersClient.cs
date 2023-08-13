using AntiClown.DiscordBot.Options;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Members;

public class MembersClient : IMembersClient
{
    public MembersClient(
        DiscordClient discordClient,
        IOptions<DiscordOptions> discordOptions
    )
    {
        this.discordClient = discordClient;
        this.discordOptions = discordOptions;
    }

    public Task<ulong> GetBotIdAsync()
    {
        return Task.FromResult(discordClient.CurrentUser.Id);
    }

    public async Task<DiscordMember[]> GetAllAsync()
    {
        return (await discordClient.Guilds[discordOptions.Value.GuildId].GetAllMembersAsync()).ToArray();
    }

    public async Task<DiscordMember> GetAsync(ulong userId)
    {
        var member = await discordClient.Guilds[discordOptions.Value.GuildId].GetMemberAsync(userId);

        return member;
    }

    public async Task ModifyAsync(DiscordMember member, Action<MemberEditModel> action)
    {
        await member.ModifyAsync(action);
    }

    private readonly DiscordClient discordClient;
    private readonly IOptions<DiscordOptions> discordOptions;
}