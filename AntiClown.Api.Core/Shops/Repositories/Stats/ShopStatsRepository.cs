using AntiClown.Api.Core.Shops.Domain;
using AutoMapper;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Shops.Repositories.Stats;

public class ShopStatsRepository : IShopStatsRepository
{
    public ShopStatsRepository(
        IVersionedSqlRepository<ShopStatsStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<ShopStats> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return mapper.Map<ShopStats>(result);
    }

    public async Task CreateAsync(ShopStats shopStats)
    {
        var storageElement = mapper.Map<ShopStatsStorageElement>(shopStats);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(ShopStats shopStats)
    {
        await sqlRepository.ConcurrentUpdateAsync(shopStats.Id, storageElement =>
        {
            storageElement.TotalReRolls = shopStats.TotalReRolls;
            storageElement.ItemsBought = shopStats.ItemsBought;
            storageElement.TotalReveals = shopStats.TotalReveals;
            storageElement.ScamCoinsLostOnReveals = shopStats.ScamCoinsLostOnReveals;
            storageElement.ScamCoinsLostOnReRolls = shopStats.ScamCoinsLostOnReRolls;
            storageElement.ScamCoinsLostOnPurchases = shopStats.ScamCoinsLostOnPurchases;
        });
    }

    private readonly IVersionedSqlRepository<ShopStatsStorageElement> sqlRepository;
    private readonly IMapper mapper;
}