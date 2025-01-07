using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Domain.MassActions;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Transactions.Domain;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Services;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Api.Core.Economies.Services;

public class EconomyService(
    IEconomyRepository economyRepository,
    IUsersService usersService,
    ITransactionsService transactionsService,
    IAntiClownDataApiClient antiClownDataApiClient
)
    : IEconomyService
{
    public async Task<Economy> ReadEconomyAsync(Guid userId)
    {
        return await economyRepository.ReadAsync(userId);
    }

    public async Task UpdateScamCoinsAsync(Guid userId, int diff, string reason)
    {
        var economy = await ReadEconomyAsync(userId);
        economy.ScamCoins += diff;
        await economyRepository.UpdateAsync(economy);
        await transactionsService.CreateAsync(
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ScamCoinDiff = diff,
                Reason = reason,
                DateTime = DateTime.UtcNow,
            }
        );
    }

    public async Task UpdateScamCoinsForAllAsync(int diff, string reason)
    {
        await economyRepository.UpdateAllAsync(
            new MassEconomyUpdate
            {
                ScamCoins = new MassScamCoinsUpdate
                {
                    ScamCoinsDiff = diff,
                    Reason = reason,
                },
            }
        );
        var users = await usersService.ReadAllAsync();
        var transactions = users.Select(
            x => new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = x.Id,
                ScamCoinDiff = diff,
                Reason = reason,
                DateTime = DateTime.UtcNow,
            }
        ).ToArray();
        await transactionsService.CreateAsync(transactions);
    }

    public async Task UpdateLootBoxesAsync(Guid userId, int diff)
    {
        var economy = await ReadEconomyAsync(userId);
        economy.LootBoxes += diff;
        await economyRepository.UpdateAsync(economy);
    }

    public async Task UpdateLohotronAsync(Guid userId, bool isReady)
    {
        var economy = await ReadEconomyAsync(userId);
        economy.IsLohotronReady = isReady;
        await economyRepository.UpdateAsync(economy);
    }

    public async Task CreateEmptyAsync(Guid userId)
    {
        var newUserEconomy = await CreateDefaultAsync(userId);
        newUserEconomy.Id = userId;
        await economyRepository.CreateAsync(newUserEconomy);
    }

    public async Task UpdateNextTributeCoolDownAsync(Guid userId, DateTime nextTribute)
    {
        var economy = await ReadEconomyAsync(userId);
        economy.NextTribute = nextTribute;
        await economyRepository.UpdateAsync(economy);
    }

    public async Task ResetAllCoolDownsAsync()
    {
        await economyRepository.UpdateAllAsync(
            new MassEconomyUpdate
            {
                NextTribute = DateTime.UtcNow,
            }
        );
    }

    public async Task ResetLohotronForAllAsync()
    {
        await economyRepository.UpdateAllAsync(
            new MassEconomyUpdate
            {
                IsLohotronReady = true,
            }
        );
    }

    private async Task<Economy> CreateDefaultAsync(Guid id)
    {
        var defaultScamCoins = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.Economy, "DefaultScamCoins");
        return new Economy
        {
            Id = id,
            ScamCoins = defaultScamCoins,
            NextTribute = DateTime.UtcNow,
            LootBoxes = 0,
            IsLohotronReady = true,
        };
    }
}