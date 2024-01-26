using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Hash;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;

public class MinecraftRegisterService : IMinecraftRegisterService
{
    private readonly IMinecraftAccountRepository minecraftAccountRepository;

    public MinecraftRegisterService(IMinecraftAccountRepository minecraftAccountRepository)
    {
        this.minecraftAccountRepository = minecraftAccountRepository;
    }

    public async Task<bool> CreateOrChangeAccountAsync(Guid discordId, string username, string password)
    {
        var account = (await minecraftAccountRepository.GetAccountsByNicknamesAsync(username)).SingleOrDefault();

        if (account == null)
        {
            await minecraftAccountRepository.CreateOrUpdateAsync(new MinecraftAccount
            {
                UserId = Guid.NewGuid(),
                Username = username,
                UsernameAndPasswordHash = HashingHelper.Hash(username + password),
                AccessTokenHash = null,
                SkinUrl = null,
                CapeUrl = null,
                DiscordId = discordId.ToString()
            });

            return true;
        }

        if (account.DiscordId != discordId.ToString())
            return false;

        account.Username = username;
        account.UsernameAndPasswordHash = HashingHelper.Hash(username + password);
        account.AccessTokenHash = null;
        account.SkinUrl = null;
        account.CapeUrl = null;

        await minecraftAccountRepository.CreateOrUpdateAsync(account);
        return true;
    }
}