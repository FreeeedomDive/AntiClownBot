using AntiClown.DiscordApi.Dto.Members;

namespace AntiClown.DiscordApi.Client.Members;

public interface IMembersClient
{
    Task<DiscordMemberDto?> GetDiscordMemberAsync(Guid userId);
}