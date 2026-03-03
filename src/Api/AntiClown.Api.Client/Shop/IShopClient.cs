/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Api.Client.Shop;

public interface IShopClient
{
    Task<AntiClown.Api.Dto.Shops.CurrentShopInfoDto> ReadAsync(System.Guid shopId);
    Task<AntiClown.Api.Dto.Shops.ShopItemDto> RevealAsync(System.Guid shopId, System.Guid itemId);
    Task<AntiClown.Api.Dto.Inventories.BaseItemDto> BuyAsync(System.Guid shopId, System.Guid itemId);
    Task ReRollAsync(System.Guid shopId);
    Task ResetAsync(System.Guid shopId);
    Task<AntiClown.Api.Dto.Shops.ShopStatsDto> ReadStatsAsync(System.Guid shopId);
    Task ResetAllAsync();
}
