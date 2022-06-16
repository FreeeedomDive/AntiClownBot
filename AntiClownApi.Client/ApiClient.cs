using AntiClownApiClient.ItemsClient;
using AntiClownApiClient.ShopClient;
using AntiClownApiClient.UsersClient;
using AntiClownApiClient.UtilityClient;
using Loggers;
using RestSharp;

namespace AntiClownApiClient;

public class ApiClient : IApiClient
{
    public ApiClient(RestClient restClient, ILogger logger)
    {
        Users = new UsersClient.UsersClient(restClient);
        Shop = new ShopClient.ShopClient(restClient, logger);
        Items = new ItemsClient.ItemsClient(restClient);
        Utility = new UtilityClient.UtilityClient(restClient);
    }

    public IUsersClient Users { get; }
    public IShopClient Shop { get; }
    public IItemsClient Items { get; }
    public IUtilityClient Utility { get; }
}