using DSharpPlus.Entities;
using DSharpPlus.Net.Models;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Members;

public interface IMembersClient
{
    Task<ulong> GetBotIdAsync();
    Task<DiscordMember[]> GetAllAsync();
    Task<DiscordMember> GetAsync(ulong userId);
    Task ModifyAsync(DiscordMember member, Action<MemberEditModel> action);
}