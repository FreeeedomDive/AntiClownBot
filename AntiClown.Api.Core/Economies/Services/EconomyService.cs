using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Domain.MassActions;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Transactions.Domain;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Services;

namespace AntiClown.Api.Core.Economies.Services;

public class EconomyService : IEconomyService
{
    public EconomyService(
        IEconomyRepository economyRepository,
        IUsersService usersService,
        ITransactionsService transactionsService
    )
    {
        this.economyRepository = economyRepository;
        this.usersService = usersService;
        this.transactionsService = transactionsService;
    }

    public async Task<Economy> ReadEconomyAsync(Guid userId)
    {
        return await economyRepository.ReadAsync(userId);
    }

    public async Task UpdateScamCoinsAsync(Guid userId, int diff, string reason)
    {
        var economy = await ReadEconomyAsync(userId);
        economy.ScamCoins += diff;
        await economyRepository.UpdateAsync(economy);
        await transactionsService.CreateAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ScamCoinDiff = diff,
            Reason = reason,
            DateTime = DateTime.UtcNow,
        });
    }

    public async Task UpdateScamCoinsForAllAsync(int diff, string reason)
    {
        await economyRepository.UpdateAllAsync(new MassEconomyUpdate
        {
            ScamCoins = new MassScamCoinsUpdate
            {
                ScamCoinsDiff = diff,
                Reason = reason,
            },
        });
        var users = await usersService.ReadAllAsync();
        var transactions = users.Select(x => new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = x.Id,
            ScamCoinDiff = diff,
            Reason = reason,
            DateTime = DateTime.UtcNow,
        }).ToArray();
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
        var newUserEconomy = Economy.Default;
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
        await economyRepository.UpdateAllAsync(new MassEconomyUpdate
        {
            NextTribute = DateTime.UtcNow,
        });
    }

    public async Task ResetLohotronForAllAsync()
    {
        await economyRepository.UpdateAllAsync(new MassEconomyUpdate
        {
            IsLohotronReady = true,
        });
    }

    private readonly IEconomyRepository economyRepository;
    private readonly IUsersService usersService;
    private readonly ITransactionsService transactionsService;
}