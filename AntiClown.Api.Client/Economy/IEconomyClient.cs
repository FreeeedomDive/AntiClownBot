using AntiClown.Api.Dto.Economies;

namespace AntiClown.Api.Client.Economy;

public interface IEconomyClient
{
    Task<EconomyDto> ReadAsync(Guid userId);
    Task UpdateScamCoinsAsync(Guid userId, int scamCoinsDiff, string reason);
    Task UpdateLootBoxesAsync(Guid userId, int lootBoxesDiff);
    Task ResetAllCoolDownsAsync();
}