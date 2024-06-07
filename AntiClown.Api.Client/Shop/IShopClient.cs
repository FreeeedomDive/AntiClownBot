/* Generated file */
namespace AntiClown.Api.Client.Shop;

public interface IShopClient
{
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Shops.CurrentShopInfoDto> ReadAsync(System.Guid shopId);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Shops.ShopItemDto> RevealAsync(System.Guid shopId, System.Guid itemId);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Inventories.BaseItemDto> BuyAsync(System.Guid shopId, System.Guid itemId);
    System.Threading.Tasks.Task ReRollAsync(System.Guid shopId);
    System.Threading.Tasks.Task ResetAsync(System.Guid shopId);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Shops.ShopStatsDto> ReadStatsAsync(System.Guid shopId);
    System.Threading.Tasks.Task ResetAllAsync();
}
