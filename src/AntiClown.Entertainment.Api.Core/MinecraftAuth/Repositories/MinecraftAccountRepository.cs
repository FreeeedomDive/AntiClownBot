using AntiClown.Entertainment.Api.Core.Mappings;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;
using AutoMapper;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;

public class MinecraftAccountRepository : IMinecraftAccountRepository
{
    private readonly IMapper mapper;
    private readonly ISqlRepository<MinecraftAccountStorageElement> sqlRepository;

    public MinecraftAccountRepository(
        ISqlRepository<MinecraftAccountStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<MinecraftAccount> ReadAsync(Guid userId)
    {
        return mapper.Map<MinecraftAccount>(await sqlRepository.ReadAsync(userId));
    }

    public async Task<MinecraftAccount[]> ReadManyAsync(Guid[] userId)
    {
        var elements = await sqlRepository.ReadAsync(userId);

        return elements.Select(x => mapper.Map<MinecraftAccount>(x)).ToArray();
    }

    public async Task<MinecraftAccount> CreateOrUpdateAsync(MinecraftAccount account)
    {
        var accountById = await sqlRepository.TryReadAsync(account.Id);

        if (accountById is null)
        {
            var accountByUsername = (await sqlRepository.FindAsync(x => x.Username == account.Username)).SingleOrDefault();
            if (accountByUsername != null)
                throw new ArgumentException("Аккаунт с таким именем уже существует");

            await sqlRepository.CreateAsync(mapper.Map<MinecraftAccountStorageElement>(account));
            return account;
        }

        await sqlRepository.UpdateAsync(account.Id, se =>
        {
            var newSe = mapper.Map<MinecraftAccountStorageElement>(account);
            se.Username = newSe.Username;
            se.UsernameAndPasswordHash = newSe.UsernameAndPasswordHash;
            se.AccessTokenHash = newSe.AccessTokenHash;
            se.SkinUrl = newSe.SkinUrl;
            se.CapeUrl = newSe.CapeUrl;
            se.DiscordId = newSe.DiscordId;
        });

        return mapper.Map<MinecraftAccount>(await sqlRepository.ReadAsync(account.Id));
    }

    public async Task DeleteAsync(Guid userId)
    {
        await sqlRepository.DeleteAsync(userId);
    }

    public async Task<MinecraftAccount[]> GetAccountsByNicknamesAsync(params string[] nicknames)
    {
        var elements = await sqlRepository.FindAsync(x => nicknames.Contains(x.Username));

        return elements.Select(x => mapper.Map<MinecraftAccount>(x)).ToArray();
    }

    public async Task<MinecraftAccount?> GetAccountByDiscordIdAsync(Guid discordId)
    {
        var account = (await sqlRepository.FindAsync(x => x.DiscordId == discordId.ToString())).SingleOrDefault();

        return account is null ? null : mapper.Map<MinecraftAccount>(account);
    }

    public async Task<MinecraftAccount[]> ReadAllAsync()
    {
        var allAccounts = await sqlRepository.ReadAllAsync();

        return allAccounts.Select(x => mapper.Map<MinecraftAccount>(x)).ToArray();
    }
}