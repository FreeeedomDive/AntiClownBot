using AntiClown.Api.Core.Shops.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Shops.Repositories.Items;

public class ShopItemsRepository(
    ISqlRepository<ShopItemStorageElement> sqlRepository,
    IMapper mapper
) : IShopItemsRepository
{
    public async Task<ShopItem?> TryReadAsync(Guid id)
    {
        var result = await sqlRepository.TryReadAsync(id);
        return mapper.Map<ShopItem?>(result);
    }

    public async Task<ShopItem[]> FindAsync(Guid shopId)
    {
        var queryable = await sqlRepository.BuildCustomQueryAsync();
        var result = queryable
                     .Where(x => x.ShopId == shopId)
                     .OrderBy(x => x.Id)
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
}