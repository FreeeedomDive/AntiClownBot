using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        return Deserialize(result);
    }

    public async Task<BaseItem[]> ReadManyAsync(Guid[] ids)
    {
        var result = await sqlRepository.ReadAsync(ids);
        return result.Select(Deserialize).ToArray();
    }

    public async Task<BaseItem[]> FindAsync(ItemsFilter filter)
    {
        var result = await sqlRepository
            .BuildCustomQuery()
            .WhereIf(filter.OwnerId.HasValue, x => x.OwnerId == filter.OwnerId)
            .WhereIf(filter.IsActive.HasValue, x => x.IsActive == filter.IsActive)
            .WhereIf(filter.Name != null, x => x.Name == filter.Name)
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
        await sqlRepository.UpdateAsync(item.Id, x => { x.IsActive = item.IsActive; });
    }

    public async Task DeleteAsync(Guid id)
    {
        await sqlRepository.DeleteAsync(id);
    }

    /// <summary>
    ///     JsonConvert.DeserializeObject не умеет преобразовывать конкретные реализации в BaseItem при десериализации и падает.
    ///     Пришлось написать свой костыль для корректной десериализации.
    ///     TODO: подумать, как автоматизировать это и по возможности вынести это в AutoMapper.
    /// </summary>
    private static BaseItem Deserialize(ItemStorageElement storageElement)
    {
        var serialized = storageElement.ItemSpecs;
        var itemName = Enum.TryParse<ItemName>(storageElement.Name, out var name)
            ? name
            : throw new InvalidOperationException($"Unexpected item name {storageElement.Name} in {nameof(ItemStorageElement)} {storageElement.Id}");
        return itemName switch
        {
            ItemName.CatWife => JsonConvert.DeserializeObject<CatWife>(serialized)!,
            ItemName.CommunismBanner => JsonConvert.DeserializeObject<CommunismBanner>(serialized)!,
            ItemName.DogWife => JsonConvert.DeserializeObject<DogWife>(serialized)!,
            ItemName.Internet => JsonConvert.DeserializeObject<Internet>(serialized)!,
            ItemName.JadeRod => JsonConvert.DeserializeObject<JadeRod>(serialized)!,
            ItemName.RiceBowl => JsonConvert.DeserializeObject<RiceBowl>(serialized)!,
            _ => throw new ArgumentOutOfRangeException(nameof(itemName))
        };
    }

    private readonly ISqlRepository<ItemStorageElement> sqlRepository;
    private readonly IMapper mapper;
}