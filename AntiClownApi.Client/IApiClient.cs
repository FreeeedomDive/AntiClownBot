using AntiClownApiClient.ItemsClient;
using AntiClownApiClient.ShopClient;
using AntiClownApiClient.UsersClient;
using AntiClownApiClient.UtilityClient;

namespace AntiClownApiClient;

public interface IApiClient
{
    IUsersClient Users { get; }
    IShopClient Shop { get; }
    IItemsClient Items { get; }
    IUtilityClient Utility { get; }
}