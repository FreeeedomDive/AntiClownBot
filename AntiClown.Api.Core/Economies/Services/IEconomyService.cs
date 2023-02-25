using AntiClown.Api.Core.Economies.Domain;

namespace AntiClown.Api.Core.Economies.Services;

public interface IEconomyService
{
    Task<Economy> ReadEconomyAsync(Guid userId);
    Task UpdateScamCoinsAsync(Guid userId, int diff, string reason);
    Task UpdateLootBoxesAsync(Guid userId, int diff);
}