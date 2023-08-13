using AntiClown.Api.Core.Economies.Domain;

namespace AntiClown.Api.Core.Economies.Services;

public interface IEconomyService
{
    Task<Economy> ReadEconomyAsync(Guid userId);
    Task UpdateScamCoinsAsync(Guid userId, int diff, string reason);
    Task UpdateScamCoinsForAllAsync(int diff, string reason);
    Task UpdateLootBoxesAsync(Guid userId, int diff);
    Task UpdateLohotronAsync(Guid userId, bool isReady);
    Task UpdateNextTributeCoolDownAsync(Guid userId, DateTime nextTribute);
    Task CreateEmptyAsync(Guid userId);
    Task ResetAllCoolDownsAsync();
    Task ResetLohotronForAllAsync();
}