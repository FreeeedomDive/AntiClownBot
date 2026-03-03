using AntiClown.Api.Core.Shops.Domain;

namespace AntiClown.Api.Core.Shops.Repositories.Stats;

public interface IShopStatsRepository
{
    Task<ShopStats> ReadAsync(Guid id);
    Task CreateAsync(ShopStats shopStats);
    Task UpdateAsync(ShopStats shopStats);
}