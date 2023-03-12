using AntiClown.Api.Core.Shops.Domain;

namespace AntiClown.Api.Core.Shops.Repositories.Shops;

public interface IShopsRepository
{
    Task<Shop> ReadAsync(Guid id);
    Task CreateAsync(Shop shop);
    Task UpdateAsync(Shop shop);
}