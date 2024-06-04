/* Generated file */
namespace AntiClown.Api.Client.Economy;

public interface IEconomyClient
{
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.EconomyDto> ReadAsync(System.Guid userId);
    System.Threading.Tasks.Task UpdateScamCoinsAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateScamCoinsDto updateScamCoinsDto);
    System.Threading.Tasks.Task UpdateScamCoinsForAllAsync(System.Int32 scamCoinsDiff, System.String reason);
    System.Threading.Tasks.Task UpdateLootBoxesAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateLootBoxesDto lootBoxesDto);
    System.Threading.Tasks.Task ResetAllCoolDownsAsync();
}
