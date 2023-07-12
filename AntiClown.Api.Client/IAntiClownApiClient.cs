using AntiClown.Api.Client.Economy;
using AntiClown.Api.Client.Inventory;
using AntiClown.Api.Client.Shop;
using AntiClown.Api.Client.Users;

namespace AntiClown.Api.Client;

public interface IAntiClownApiClient
{
    IUsersClient Users { get; }
    IEconomyClient Economy { get; }
    IInventoryClient Inventories { get; }
    IShopClient Shops { get; }
    ITransactionsClient Transactions { get; }
    ITributeClient Tribute { get; }
    ILohotronClient Lohotron { get; }
}