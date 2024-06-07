/* Generated file */

using AntiClown.Api.Client.Economy;
using AntiClown.Api.Client.Inventory;
using AntiClown.Api.Client.Lohotron;
using AntiClown.Api.Client.Shop;
using AntiClown.Api.Client.Transactions;
using AntiClown.Api.Client.Tributes;
using AntiClown.Api.Client.Users;

namespace AntiClown.Api.Client;

public interface IAntiClownApiClient
{
    IEconomyClient Economy { get; }
    IInventoryClient Inventory { get; }
    ILohotronClient Lohotron { get; }
    IShopClient Shop { get; }
    ITransactionsClient Transactions { get; }
    ITributesClient Tributes { get; }
    IUsersClient Users { get; }
}
