/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Api.Client.Economy;

public interface IEconomyClient
{
    Task<AntiClown.Api.Dto.Economies.EconomyDto> ReadAsync(System.Guid userId);
    Task UpdateScamCoinsAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateScamCoinsDto updateScamCoinsDto);
    Task UpdateScamCoinsForAllAsync(int scamCoinsDiff, string reason);
    Task UpdateLootBoxesAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateLootBoxesDto lootBoxesDto);
    Task ResetAllCoolDownsAsync();
}
