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
    public AntiClownApiClient(RestSharp.RestClient client)
    {
        Economy = new EconomyClient(client);
        Inventory = new InventoryClient(client);
        Lohotron = new LohotronClient(client);
        Shop = new ShopClient(client);
        Transactions = new TransactionsClient(client);
        Tributes = new TributesClient(client);
        Users = new UsersClient(client);
    }

    public IEconomyClient Economy { get; }
    public IInventoryClient Inventory { get; }
    public ILohotronClient Lohotron { get; }
    public IShopClient Shop { get; }
    public ITransactionsClient Transactions { get; }
    public ITributesClient Tributes { get; }
    public IUsersClient Users { get; }
}
