﻿using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Transactions.Services;

namespace AntiClown.Api.Core.Economies.Services;

public class EconomyService : IEconomyService
{
    public EconomyService(
        IEconomyRepository economyRepository,
        ITransactionsService transactionsService
    )
    {
        this.economyRepository = economyRepository;
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
    }

    public async Task UpdateLootBoxesAsync(Guid userId, int diff)
    {
        var economy = await ReadEconomyAsync(userId);
        economy.LootBoxes += diff;
        await economyRepository.UpdateAsync(economy);
    }

    public async Task CreateEmptyAsync(Guid userId)
    {
        var newUserEconomy = Economy.Default;
        newUserEconomy.Id = userId;
        await economyRepository.CreateAsync(newUserEconomy);
    }

    private readonly IEconomyRepository economyRepository;
    private readonly ITransactionsService transactionsService;
}