using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Domain.Items;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Core.Options;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Core.Shops.Repositories.Shops;
using AntiClown.Api.Core.Shops.Repositories.Stats;
using AntiClown.Api.Core.Shops.Services;
using AntiClown.Api.Core.Transactions.Domain;
using AntiClown.Api.Core.Transactions.Repositories;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Repositories;
using AntiClown.Api.Core.Users.Services;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SqlRepositoryBase.Core.Repository;

// connection strings example
const string oldDatabaseConnectionString = "Host=localhost;Port=5432;Database=OldAntiClown;Username=postgres;Password=postgres;Include Error Detail=true";
const string newDatabaseConnectionString = "Host=localhost;Port=5432;Database=AntiClownApi;Username=postgres;Password=postgres;Include Error Detail=true";

var oldDatabaseContext = new DatabaseContext(
    new DbContextOptions<DatabaseContext>(), new OptionsWrapper<DbOptions>(
        new DbOptions
        {
            ConnectionString = oldDatabaseConnectionString,
        }
    )
);
var allUsers = await oldDatabaseContext.Users
                                       .Include(u => u.Economy)
                                       .ThenInclude(e => e.Transactions)
                                       .Include(u => u.Items)
                                       .ThenInclude(i => i.ItemStats)
                                       .ToArrayAsync();

Console.WriteLine("Read old database");
var newDatabaseContext = new AntiClown.Api.Core.Database.DatabaseContext(
    new DbContextOptions<AntiClown.Api.Core.Database.DatabaseContext>(), new OptionsWrapper<DatabaseOptions>(
        new DatabaseOptions
        {
            ConnectionString = newDatabaseConnectionString,
        }
    )
);
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddMaps(assemblies));
var mapper = mapperConfiguration.CreateMapper();

var usersSqlRepository = new SqlRepository<UserStorageElement>(newDatabaseContext);
var usersRepository = new UsersRepository(usersSqlRepository, mapper);
var economiesSqlRepository = new VersionedSqlRepository<EconomyStorageElement>(newDatabaseContext);
var economiesRepository = new EconomyRepository(economiesSqlRepository, mapper);
var transactionsSqlRepository = new SqlRepository<TransactionStorageElement>(newDatabaseContext);
var transactionsRepository = new TransactionsRepository(transactionsSqlRepository, mapper);
var itemsSqlRepository = new SqlRepository<ItemStorageElement>(newDatabaseContext);
var itemsRepository = new ItemsRepository(itemsSqlRepository, mapper);
var shopsSqlRepository = new VersionedSqlRepository<ShopStorageElement>(newDatabaseContext);
var shopsRepository = new ShopsRepository(shopsSqlRepository, mapper);
var shopItemsSqlRepository = new SqlRepository<ShopItemStorageElement>(newDatabaseContext);
var shopItemsRepository = new ShopItemsRepository(shopItemsSqlRepository, mapper);
var shopStatsSqlRepository = new VersionedSqlRepository<ShopStatsStorageElement>(newDatabaseContext);
var shopStatsRepository = new ShopStatsRepository(shopStatsSqlRepository, mapper);
var usersService = new UsersService(usersRepository);
var transactionsService = new TransactionsService(transactionsRepository);
var economyService = new EconomyService(economiesRepository, usersService, transactionsService);
var itemsService = new ItemsService(
    new ItemsValidator(economyService, itemsRepository),
    itemsRepository,
    economyService
);
var shopsService = new ShopsService(
    shopsRepository,
    shopItemsRepository,
    shopStatsRepository,
    new ShopsValidator(economyService, shopItemsRepository),
    economyService,
    itemsService,
    mapper
);
var newUserService = new NewUserService(usersRepository, economyService, shopsService, mapper);

Console.WriteLine("Built all dependencies");
foreach (var user in allUsers)
{
    var userId = await newUserService.CreateNewUserAsync(
        new NewUser
        {
            DiscordId = user.DiscordId,
        }
    );
    Console.WriteLine($"Created user {userId} for {user.DiscordId}");
    var transactions = user.Economy.Transactions.Select(
        x => new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ScamCoinDiff = x.RatingChange,
            DateTime = x.DateTime.ToUniversalTime(),
            Reason = x.Description,
        }
    ).ToArray();
    await transactionsService.CreateAsync(transactions);
    Console.WriteLine($"Created {transactions.Length} transactions");
    var economy = new Economy
    {
        Id = userId,
        ScamCoins = user.Economy.ScamCoins,
        LootBoxes = user.Economy.LootBoxes,
        NextTribute = user.Economy.NextTribute.ToUniversalTime(),
        IsLohotronReady = true,
    };
    await economiesRepository.UpdateAsync(economy);
    Console.WriteLine($"Updated {economy.Id} economy");
    var items = user.Items.Select(
        x => x.Name switch
        {
            StringConstants.CatWifeName => (BaseItem)new CatWife
            {
                Id = Guid.NewGuid(),
                Rarity = Enum.Parse<Rarity>(x.Rarity.ToString()),
                IsActive = x.IsActive,
                OwnerId = userId,
                Price = x.Price,
                AutoTributeChance = x.ItemStats.CatAutoTributeChance,
            },
            StringConstants.CommunismBannerName => new CommunismBanner
            {
                Id = Guid.NewGuid(),
                Rarity = Enum.Parse<Rarity>(x.Rarity.ToString()),
                IsActive = x.IsActive,
                OwnerId = userId,
                Price = x.Price,
                DivideChance = x.ItemStats.CommunismDivideChance,
                StealChance = x.ItemStats.CommunismStealChance,
            },
            StringConstants.DogWifeName => new DogWife
            {
                Id = Guid.NewGuid(),
                Rarity = Enum.Parse<Rarity>(x.Rarity.ToString()),
                IsActive = x.IsActive,
                OwnerId = userId,
                Price = x.Price,
                LootBoxFindChance = x.ItemStats.DogLootBoxFindChance,
            },
            StringConstants.InternetName => new Internet
            {
                Id = Guid.NewGuid(),
                Rarity = Enum.Parse<Rarity>(x.Rarity.ToString()),
                IsActive = x.IsActive,
                OwnerId = userId,
                Price = x.Price,
                Gigabytes = x.ItemStats.InternetGigabytes,
                Ping = x.ItemStats.InternetPing,
                Speed = x.ItemStats.InternetSpeed,
            },
            StringConstants.JadeRodName => new JadeRod
            {
                Id = Guid.NewGuid(),
                Rarity = Enum.Parse<Rarity>(x.Rarity.ToString()),
                IsActive = x.IsActive,
                OwnerId = userId,
                Price = x.Price,
                Length = x.ItemStats.JadeRodLength,
                Thickness = x.ItemStats.JadeRodThickness,
            },
            StringConstants.RiceBowlName => new RiceBowl
            {
                Id = Guid.NewGuid(),
                Rarity = Enum.Parse<Rarity>(x.Rarity.ToString()),
                IsActive = x.IsActive,
                OwnerId = userId,
                Price = x.Price,
                NegativeRangeExtend = x.ItemStats.RiceNegativeRangeExtend,
                PositiveRangeExtend = x.ItemStats.RicePositiveRangeExtend,
            },
        }
    ).ToArray();
    for (var i = 0; i < items.Length; i++)
    {
        var item = items[i];
        await itemsRepository.CreateAsync(item);
        Console.WriteLine($"Created item {i + 1}");
    }
}