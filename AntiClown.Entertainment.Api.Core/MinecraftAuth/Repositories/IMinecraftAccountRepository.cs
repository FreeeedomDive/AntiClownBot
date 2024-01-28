using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;

namespace AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;

public interface IMinecraftAccountRepository
{
    Task<MinecraftAccount> ReadAsync(Guid userId);

    Task<MinecraftAccount[]> ReadManyAsync(Guid[] userId);

    Task<MinecraftAccount> CreateOrUpdateAsync(MinecraftAccount account);

    Task DeleteAsync(Guid userId);

    Task<MinecraftAccount[]> GetAccountsByNicknamesAsync(params string[] nicknames);

    Task<MinecraftAccount?> GetAccountByDiscordId(Guid discordId);
}