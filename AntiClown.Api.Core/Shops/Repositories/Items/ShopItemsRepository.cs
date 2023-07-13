using AntiClown.Api.Core.Shops.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Shops.Repositories.Items;

public class ShopItemsRepository : IShopItemsRepository
{
    public ShopItemsRepository(
        ISqlRepository<ShopItemStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<ShopItem?> TryReadAsync(Guid id)
    {
        var result = await sqlRepository.TryReadAsync(id);
        return mapper.Map<ShopItem?>(result);
    }

    public async Task<ShopItem[]> FindAsync(Guid shopId)
    {
        var result = await sqlRepository
                           .BuildCustomQuery()
                           .Where(x => x.ShopId == shopId)
                           .ToArrayAsync();
        return mapper.Map<ShopItem[]>(result);
    }

    public async Task CreateManyAsync(ShopItem[] items)
    {
        var storageElements = mapper.Map<ShopItemStorageElement[]>(items);
        await sqlRepository.CreateAsync(storageElements);
    }

    public async Task UpdateAsync(ShopItem item)
    {
        await sqlRepository.UpdateAsync(
            item.Id, x =>
            {
                x.IsRevealed = item.IsRevealed;
                x.IsOwned = item.IsOwned;
            }
        );
    }

    public async Task DeleteManyAsync(Guid[] ids)
    {
        await sqlRepository.DeleteAsync(ids);
    }

    private readonly IMapper mapper;

    private readonly ISqlRepository<ShopItemStorageElement> sqlRepository;
}