using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Inventory.Repositories;

public class ItemsRepository(
    ISqlRepository<ItemStorageElement> sqlRepository,
    IMapper mapper
) : IItemsRepository
{
    public async Task<BaseItem> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return Deserialize(result);
    }

    public async Task<BaseItem[]> ReadManyAsync(Guid[] ids)
    {
        var result = await sqlRepository.ReadAsync(ids);
        return result.Select(Deserialize).ToArray();
    }

    public async Task<BaseItem[]> FindAsync(ItemsFilter filter)
    {
        var queryable = await sqlRepository.BuildCustomQueryAsync();
        var result = await queryable.WhereIf(filter.OwnerId.HasValue, x => x.OwnerId == filter.OwnerId)
                                    .WhereIf(filter.IsActive.HasValue, x => x.IsActive == filter.IsActive)
                                    .WhereIf(filter.Name != null, x => x.Name == filter.Name)
                                    .OrderBy(x => x.Id)
                                    .ToArrayAsync();
        return result.Select(Deserialize).ToArray();
    }

    public async Task CreateAsync(BaseItem item)
    {
        var storageElement = mapper.Map<ItemStorageElement>(item);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(BaseItem item)
    {
        var newStorageElement = mapper.Map<ItemStorageElement>(item);
        await sqlRepository.UpdateAsync(
            item.Id, x =>
            {
                x.OwnerId = newStorageElement.OwnerId;
                x.IsActive = newStorageElement.IsActive;
                x.ItemSpecs = newStorageElement.ItemSpecs;
            }
        );
    }

    public async Task DeleteAsync(Guid id)
    {
        await sqlRepository.DeleteAsync(id);
    }

    private static BaseItem Deserialize(ItemStorageElement storageElement)
    {
        var serialized = storageElement.ItemSpecs;
        return JsonConvert.DeserializeObject<BaseItem>(
            serialized, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }
        )!;
    }
}