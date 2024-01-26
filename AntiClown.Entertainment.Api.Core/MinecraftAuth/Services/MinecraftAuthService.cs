using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Hash;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;

public class MinecraftAuthService : IMinecraftAuthService
{
    private readonly IMinecraftAccountRepository minecraftAccountRepository;

    public MinecraftAuthService(IMinecraftAccountRepository minecraftAccountRepository)
    {
        this.minecraftAccountRepository = minecraftAccountRepository;
    }

    public async Task<AuthResponse?> Auth(string username, string password)
    {
        var account = (await minecraftAccountRepository.GetAccountsByNicknamesAsync(username)).FirstOrDefault();
        if (account is null)
            return null;

        var hashedAuthData = HashingHelper.Hash(username + password);
        if (account.UsernameAndPasswordHash != hashedAuthData)
            return null;

        var accessToken = Guid.NewGuid();
        account.AccessTokenHash = HashingHelper.Hash(accessToken.ToString());
        await minecraftAccountRepository.CreateOrUpdateAsync(account);

        return new AuthResponse
        {
            AccessToken = accessToken.ToString(),
            Username = username,
            UserId = account.UserId.ToString()
        };
    }

    public async Task<bool> Join(string accessToken, string userId, string serverId)
    {
        var account = await minecraftAccountRepository.ReadAsync(Guid.Parse(userId));
        var hashedAccessToken = HashingHelper.Hash(accessToken);

        return account.AccessTokenHash == hashedAccessToken;
    }

    public async Task<HasJoinedResponse> HasJoined(string username, string serverId)
    {
        var account = (await minecraftAccountRepository.GetAccountsByNicknamesAsync(username)).FirstOrDefault();
        if (account == null)
            return new HasJoinedResponse
            {
                UserId = null,
                SkinUrl = null,
                CapeUrl = null
            };

        return new HasJoinedResponse
        {
            UserId = account.UserId.ToString(),
            SkinUrl = "https://s.namemc.com/i/fc31cc8d6fceca7b.png",
            CapeUrl = "https://s.namemc.com/i/fc31cc8d6fceca7b.png"
        };
    }

    public async Task<ProfileResponse> Profile(string userId)
    {
        var account = await minecraftAccountRepository.ReadAsync(Guid.Parse(userId));

        return new ProfileResponse
        {
            Name = account.Username,
            SkinUrl = "https://s.namemc.com/i/fc31cc8d6fceca7b.png",
            CapeUrl = "https://s.namemc.com/i/fc31cc8d6fceca7b.png"
        };
    }

    public async Task<IEnumerable<ProfilesResponse>> Profiles(string[] usernames)
    {
        var accounts = await minecraftAccountRepository.GetAccountsByNicknamesAsync(usernames);

        return accounts.Select(x => new ProfilesResponse
        {
            Id = x.UserId.ToString(),
            Name = x.Username
        });
    }
}