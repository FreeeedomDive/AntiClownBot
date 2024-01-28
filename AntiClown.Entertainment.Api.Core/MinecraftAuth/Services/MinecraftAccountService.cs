using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Hash;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Services;

public class MinecraftAccountService : IMinecraftAccountService
{
    private readonly IMinecraftAccountRepository minecraftAccountRepository;

    public MinecraftAccountService(IMinecraftAccountRepository minecraftAccountRepository)
    {
        this.minecraftAccountRepository = minecraftAccountRepository;
    }

    public async Task<RegistrationStatus> CreateOrChangeAccountAsync(Guid discordId, string username, string password)
    {
        var account = await minecraftAccountRepository.GetAccountByDiscordIdAsync(discordId);
        var accountByNickname = (await minecraftAccountRepository.GetAccountsByNicknamesAsync(username)).SingleOrDefault();

        if (accountByNickname is not null && account is null)
        {
            return RegistrationStatus.FailedCreate_NicknameOwnedByOtherUser;
        }
        
        if (accountByNickname is not null && account!.DiscordId != accountByNickname.DiscordId)
        {
            return RegistrationStatus.FailedUpdate_NicknameOwnedByOtherUser;
        }

        if (account == null)
        {
            await minecraftAccountRepository.CreateOrUpdateAsync(new MinecraftAccount
            {
                Id = Guid.NewGuid(),
                Username = username,
                UsernameAndPasswordHash = HashingHelper.Hash(username + password),
                AccessTokenHash = null,
                SkinUrl = null,
                CapeUrl = null,
                DiscordId = discordId.ToString()
            });

            return RegistrationStatus.SuccessCreate;
        }

        account.Username = username;

        var loginPasswordHash = HashingHelper.Hash(username + password);
        if (loginPasswordHash != account.UsernameAndPasswordHash)
        {
            account.UsernameAndPasswordHash = loginPasswordHash;
            account.AccessTokenHash = null;
        }

        await minecraftAccountRepository.CreateOrUpdateAsync(account);

        return RegistrationStatus.SuccessUpdate;
    }

    public async Task<bool> SetSkinAsync(Guid discordId, string? skinUrl, string? capeUrl)
    {
        var account = await minecraftAccountRepository.GetAccountByDiscordIdAsync(discordId);

        if (account == null)
            return false;

        account.SkinUrl = skinUrl ?? account.SkinUrl;
        account.CapeUrl = capeUrl ?? account.CapeUrl;
        
        await minecraftAccountRepository.CreateOrUpdateAsync(account);

        return true;
    }

    public async Task<string[]> GetAllNicknames()
    {
        var accounts = await minecraftAccountRepository.ReadAllAsync();

        return accounts.Select(x => x.Username).ToArray();
    }

    public async Task<bool> HasRegistrationByDiscordUser(Guid discordUserId)
    {
        var account = await minecraftAccountRepository.GetAccountByDiscordIdAsync(discordUserId);

        return account != null;
    }
}