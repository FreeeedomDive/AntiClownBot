﻿using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.IntegrationTests.Mocks;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Core.Shops.Repositories.Shops;
using AntiClown.Api.Core.Shops.Repositories.Stats;
using AntiClown.Api.Core.Shops.Services;
using AntiClown.Api.Core.Transactions.Repositories;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Repositories;
using AntiClown.Api.Core.Users.Services;
using AutoFixture;
using AutoMapper;
using Moq;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.IntegrationTests;

public class IntegrationTestsBase
{
    [OneTimeSetUp]
    public void SetUpDependencies()
    {
        // Build all dependencies
        Fixture = new Fixture();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddMaps(assemblies));
        var mapper = mapperConfiguration.CreateMapper();

        var databaseContext = new DatabaseContext();

        var usersSqlRepository = new SqlRepository<UserStorageElement>(databaseContext);
        var usersRepository = new UsersRepository(usersSqlRepository, mapper);

        var economiesSqlRepository = new VersionedSqlRepository<EconomyStorageElement>(databaseContext);
        var economiesRepository = new EconomyRepository(economiesSqlRepository, mapper);

        var transactionsSqlRepository = new SqlRepository<TransactionStorageElement>(databaseContext);
        var transactionsRepository = new TransactionsRepository(transactionsSqlRepository, mapper);

        var itemsSqlRepository = new SqlRepository<ItemStorageElement>(databaseContext);
        var itemsRepository = new ItemsRepository(itemsSqlRepository, mapper);

        var shopsSqlRepository = new VersionedSqlRepository<ShopStorageElement>(databaseContext);
        ShopsRepository = new ShopsRepository(shopsSqlRepository, mapper);

        var shopItemsSqlRepository = new SqlRepository<ShopItemStorageElement>(databaseContext);
        ShopItemsRepository = new ShopItemsRepository(shopItemsSqlRepository, mapper);

        var shopStatsSqlRepository = new VersionedSqlRepository<ShopStatsStorageElement>(databaseContext);
        ShopStatsRepository = new ShopStatsRepository(shopStatsSqlRepository, mapper);

        UsersService = new UsersService(usersRepository);
        TransactionsService = new TransactionsService(transactionsRepository);
        EconomyService = new EconomyService(economiesRepository, TransactionsService);
        LohotronRewardGeneratorMock = new Mock<ILohotronRewardGenerator>();
        LohotronService = new LohotronService(EconomyService, LohotronRewardGeneratorMock.Object);
        ItemsService = new ItemsService(
            new ItemsValidator(EconomyService, itemsRepository),
            itemsRepository,
            EconomyService
        );
        ShopsService = new ShopsService(
            ShopsRepository,
            ShopItemsRepository,
            ShopStatsRepository,
            new ShopsValidator(EconomyService, ShopItemsRepository),
            EconomyService,
            ItemsService,
            mapper
        );
        NewUserService = new NewUserService(usersRepository, EconomyService, ShopsService, mapper);
        Scheduler = new SchedulerMock();
        TributeService = new TributeService(EconomyService, ItemsService, new TributeMessageProducerMock(), Scheduler);
    }

    [SetUp]
    public async Task SetUp()
    {
        var newUserId = await NewUserService.CreateNewUserAsync(new NewUser
        {
            DiscordId = CreateUniqueUlong()
        });
        User = await UsersService.ReadAsync(newUserId);
    }

    protected ulong CreateUniqueUlong()
    {
        return Enumerable.Range(1, 8)
            .Select(_ => Fixture.Create<ulong>())
            .Aggregate((accumulate, x) => accumulate * x);
    }

    protected IUsersService UsersService { get; private set; } = null!;
    protected INewUserService NewUserService { get; private set; } = null!;
    protected ITransactionsService TransactionsService { get; private set; } = null!;
    protected IEconomyService EconomyService { get; private set; } = null!;
    protected Mock<ILohotronRewardGenerator> LohotronRewardGeneratorMock { get; private set; } = null!;
    protected ILohotronService LohotronService { get; private set; } = null!;
    protected IItemsService ItemsService { get; private set; } = null!;
    protected IShopsRepository ShopsRepository { get; private set; } = null!;
    protected IShopItemsRepository ShopItemsRepository { get; private set; } = null!;
    protected IShopStatsRepository ShopStatsRepository { get; private set; } = null!;
    protected IShopsService ShopsService { get; private set; } = null!;
    protected SchedulerMock Scheduler { get; private set; } = null!;
    protected ITributeService TributeService { get; private set; } = null!;
    protected IFixture Fixture { get; private set; } = null!;
    protected User User { get; private set; } = null!;
}