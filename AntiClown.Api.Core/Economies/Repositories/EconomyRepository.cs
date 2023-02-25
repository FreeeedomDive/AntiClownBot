using AntiClown.Api.Core.Economies.Domain;
using AutoMapper;
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
        await sqlRepository.ConcurrentUpdateAsync(economy.Id, x =>
        {
            x.ScamCoins = economy.ScamCoins;
            x.NextTribute = economy.NextTribute;
            x.LootBoxes = economy.LootBoxes;
        });
    }

    private readonly IVersionedSqlRepository<EconomyStorageElement> sqlRepository;
    private readonly IMapper mapper;
}