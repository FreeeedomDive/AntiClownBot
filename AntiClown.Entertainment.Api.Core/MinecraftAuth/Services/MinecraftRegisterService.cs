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

    public async Task<RegistrationStatus> CreateOrChangeAccountAsync(Guid discordId, string username, string password)
    {
        var account = await minecraftAccountRepository.GetAccountByDiscordId(discordId);
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
}