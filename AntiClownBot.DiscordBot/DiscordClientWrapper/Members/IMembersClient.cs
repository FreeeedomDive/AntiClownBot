using DSharpPlus.Entities;
using DSharpPlus.Net.Models;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Members;

public interface IMembersClient
{
    Task<ulong> GetBotIdAsync();
    Task<DiscordMember> GetAsync(ulong userId);
    Task ModifyAsync(DiscordMember member, Action<MemberEditModel> action);
}