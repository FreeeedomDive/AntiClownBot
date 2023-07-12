using AntiClown.Api.Client.Economy;
using AntiClown.Api.Client.Inventory;
using AntiClown.Api.Client.Shop;
using AntiClown.Api.Client.Users;
using RestSharp;

namespace AntiClown.Api.Client;

public class AntiClownApiClient : IAntiClownApiClient
{
    public AntiClownApiClient(RestClient restClient)
    {
        Users = new UsersClient(restClient);
        Economy = new EconomyClient(restClient);
        Inventories = new InventoryClient(restClient);
        Shops = new ShopClient(restClient);
        Transactions = new TransactionsClient(restClient);
        Tribute = new TributeClient(restClient);
        Lohotron = new LohotronClient(restClient);
    }

    public IUsersClient Users { get; }
    public IEconomyClient Economy { get; }
    public IInventoryClient Inventories { get; }
    public IShopClient Shops { get; }
    public ITransactionsClient Transactions { get; }
    public ITributeClient Tribute { get; }
    public ILohotronClient Lohotron { get; set; }
}