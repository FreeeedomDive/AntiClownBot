using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;

namespace AntiClown.Api.Core.Inventory.Repositories;

public interface IItemsRepository
{
    Task<BaseItem> ReadAsync(Guid id);
    Task<BaseItem[]> ReadManyAsync(Guid[] ids);
    Task<BaseItem[]> FindAsync(ItemsFilter filter);
    Task CreateAsync(BaseItem item);
    Task UpdateAsync(BaseItem item);
    Task DeleteAsync(Guid id);
}