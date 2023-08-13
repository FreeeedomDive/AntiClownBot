using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Dto.Exceptions.Economy;

namespace AntiClown.Api.Core.Economies.Services;

public class LohotronService : ILohotronService
{
    public LohotronService(
        IEconomyService economyService,
        ILohotronRewardGenerator lohotronRewardGenerator
    )
    {
        this.economyService = economyService;
        this.lohotronRewardGenerator = lohotronRewardGenerator;
    }

    public async Task<LohotronReward> UseLohotronAsync(Guid userId)
    {
        var economy = await economyService.ReadEconomyAsync(userId);
        if (!economy.IsLohotronReady)
        {
            throw new LohotronAlreadyUsedException(userId);
        }

        var reward = lohotronRewardGenerator.Generate();
        switch (reward.RewardType)
        {
            case LohotronRewardType.Nothing:
                break;
            case LohotronRewardType.ScamCoins:
                await economyService.UpdateScamCoinsAsync(userId, reward.ScamCoinsReward ?? 0, "Лохотрон");
                break;
            case LohotronRewardType.LootBox:
                await economyService.UpdateLootBoxesAsync(userId, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(reward.RewardType), "Unknown lohotron reward type");
        }

        await economyService.UpdateLohotronAsync(userId, false);
        return reward;
    }

    public async Task ResetLohotronForAllAsync()
    {
        await economyService.ResetLohotronForAllAsync();
    }

    private readonly IEconomyService economyService;
    private readonly ILohotronRewardGenerator lohotronRewardGenerator;
}