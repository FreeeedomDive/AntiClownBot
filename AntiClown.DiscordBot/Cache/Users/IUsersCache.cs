using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Cache.Users;

public interface IUsersCache
{
    Task InitializeAsync();
    Task<DiscordMember?> GetMemberByApiIdAsync(Guid userId);
    Task<Guid> GetApiIdByMemberIdAsync(ulong memberId);
}