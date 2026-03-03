using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;

public interface IMinecraftAuthService
{
    Task<AuthResponse?> Auth(string username, string password);

    Task<bool> Join(string accessToken, string userId, string serverId);

    Task<HasJoinedResponse?> HasJoined(string username, string serverId);

    Task<ProfileResponse> Profile(string userId);

    Task<IEnumerable<ProfilesResponse>> Profiles(string[] usernames);
}