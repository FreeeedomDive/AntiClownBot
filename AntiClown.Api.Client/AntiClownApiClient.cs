/* Generated file */

using AntiClown.Api.Client.Economy;
using AntiClown.Api.Client.Inventory;
using AntiClown.Api.Client.Lohotron;
using AntiClown.Api.Client.Shop;
using AntiClown.Api.Client.Transactions;
using AntiClown.Api.Client.Tributes;
using AntiClown.Api.Client.Users;

namespace AntiClown.Api.Client;

public class AntiClownApiClient : IAntiClownApiClient
{
    public AntiClownApiClient(RestSharp.RestClient restClient)
    {
        Economy = new EconomyClient(restClient);
        Inventory = new InventoryClient(restClient);
        Lohotron = new LohotronClient(restClient);
        Shop = new ShopClient(restClient);
        Transactions = new TransactionsClient(restClient);
        Tributes = new TributesClient(restClient);
        Users = new UsersClient(restClient);
    }

    public IEconomyClient Economy { get; }
    public IInventoryClient Inventory { get; }
    public ILohotronClient Lohotron { get; }
    public IShopClient Shop { get; }
    public ITransactionsClient Transactions { get; }
    public ITributesClient Tributes { get; }
    public IUsersClient Users { get; }
}
