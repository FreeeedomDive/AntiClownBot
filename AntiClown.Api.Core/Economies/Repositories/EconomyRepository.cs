using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Domain.MassActions;
using AntiClown.Tools.Utility.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Economies.Repositories;

public class EconomyRepository : IEconomyRepository
{
    public EconomyRepository(
        IVersionedSqlRepository<EconomyStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<Economy> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return mapper.Map<Economy>(result);
    }

    public async Task CreateAsync(Economy economy)
    {
        var storageElement = mapper.Map<EconomyStorageElement>(economy);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(Economy economy)
    {
        var current = await sqlRepository.ReadAsync(economy.Id);
        await sqlRepository.ConcurrentUpdateAsync(economy.Id, x =>
        {
            x.ScamCoins = economy.ScamCoins;
            x.NextTribute = economy.NextTribute;
            x.LootBoxes = economy.LootBoxes;
            x.Version = current.Version;
        });
    }

    public async Task UpdateAllAsync(MassEconomyUpdate massEconomyUpdate)
    {
        await sqlRepository.ModifyDbSetAsync(async set =>
        {
            var economies = await set.ToArrayAsync();
            economies.ForEach(x =>
            {
                if (massEconomyUpdate.ScamCoins is not null)
                {
                    x.ScamCoins += massEconomyUpdate.ScamCoins.ScamCoinsDiff;
                }

                if (massEconomyUpdate.LootBoxesDiff is not null)
                {
                    x.LootBoxes += massEconomyUpdate.LootBoxesDiff.Value;
                }

                if (massEconomyUpdate.NextTribute is not null)
                {
                    x.NextTribute = massEconomyUpdate.NextTribute.Value;
                }
            });
        });
    }

    private readonly IVersionedSqlRepository<EconomyStorageElement> sqlRepository;
    private readonly IMapper mapper;
}