using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Api.Core.Options;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Core.Shops.Repositories.Shops;
using AntiClown.Api.Core.Shops.Repositories.Stats;
using AntiClown.Api.Core.Transactions.Repositories;
using AntiClown.Api.Core.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.ContextBuilders;

namespace AntiClown.Api.Core.Database;

public class DatabaseContext : PostgreSqlDbContext
{
    public DatabaseContext(string connectionString) : base(connectionString)
    {
    }

    public DbSet<UserStorageElement> Users { get; set; }
    public DbSet<EconomyStorageElement> Economies { get; set; }
    public DbSet<TransactionStorageElement> Transactions { get; set; }
    public DbSet<ItemStorageElement> Items { get; set; }
    public DbSet<ShopStorageElement> Shops { get; set; }
    public DbSet<ShopItemStorageElement> ShopItems { get; set; }
    public DbSet<ShopStatsStorageElement> ShopStats { get; set; }
    private DatabaseOptions Options { get; }
}