using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;

public interface IMinecraftAccountRepository
{
    public Task<MinecraftAccount> ReadAsync(Guid userId);

    public Task<MinecraftAccount[]> ReadManyAsync(Guid[] userId);

    public Task<MinecraftAccount> CreateOrUpdateAsync(MinecraftAccount account);

    public Task DeleteAsync(Guid userId);

    public Task<MinecraftAccount[]> GetAccountsByNicknamesAsync(params string[] nicknames);
}