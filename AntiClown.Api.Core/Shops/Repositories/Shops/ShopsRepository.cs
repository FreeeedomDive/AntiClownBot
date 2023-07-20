using AntiClown.Api.Core.Shops.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Shops.Repositories.Shops;

public class ShopsRepository : IShopsRepository
{
    public ShopsRepository(
        IVersionedSqlRepository<ShopStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<Shop> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return mapper.Map<Shop>(result);
    }

    public async Task CreateAsync(Shop shop)
    {
        var storageElement = mapper.Map<ShopStorageElement>(shop);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(Shop shop)
    {
        await sqlRepository.ConcurrentUpdateAsync(
            shop.Id, x =>
            {
                x.ReRollPrice = shop.ReRollPrice;
                x.FreeReveals = shop.FreeReveals;
            }
        );
    }

    public async Task ResetAllAsync()
    {
        await sqlRepository.ModifyDbSetAsync(
            async set =>
            {
                var shops = await set.ToArrayAsync();
                foreach (var shop in shops)
                {
                    shop.ReRollPrice = Shop.Default.ReRollPrice;
                    shop.FreeReveals = Shop.Default.FreeReveals;
                }
            }
        );
    }

    private readonly IMapper mapper;

    private readonly IVersionedSqlRepository<ShopStorageElement> sqlRepository;
}