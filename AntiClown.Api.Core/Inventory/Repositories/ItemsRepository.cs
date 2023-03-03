using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Inventory.Repositories;

public class ItemsRepository : IItemsRepository
{
    public ItemsRepository(
        ISqlRepository<ItemStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<BaseItem> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return mapper.Map<BaseItem>(result);
    }

    public async Task<BaseItem[]> ReadManyAsync(Guid[] ids)
    {
        var result = await sqlRepository.ReadAsync(ids);
        return mapper.Map<BaseItem[]>(result);
    }

    public async Task<BaseItem[]> FindAsync(ItemsFilter filter)
    {
        var result = await sqlRepository
            .BuildCustomQuery()
            .WhereIf(filter.OwnerId.HasValue, x => x.OwnerId == filter.OwnerId)
            .WhereIf(filter.IsActive.HasValue, x => x.IsActive == filter.IsActive)
            .WhereIf(filter.Name != null, x => x.Name == filter.Name)
            .ToArrayAsync();
        return mapper.Map<BaseItem[]>(result);
    }

    public async Task CreateAsync(BaseItem item)
    {
        var storageElement = mapper.Map<ItemStorageElement>(item);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(BaseItem item)
    {
        await sqlRepository.UpdateAsync(item.Id, x =>
        {
            x.IsActive = item.IsActive;
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        await sqlRepository.DeleteAsync(id);
    }

    private readonly ISqlRepository<ItemStorageElement> sqlRepository;
    private readonly IMapper mapper;
}