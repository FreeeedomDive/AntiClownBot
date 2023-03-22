using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Economies.Services;
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
using SqlRepositoryBase.Core.Repository;

namespace IntegrationTests;

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
        var shopsRepository = new ShopsRepository(shopsSqlRepository, mapper);

        var shopItemsSqlRepository = new SqlRepository<ShopItemStorageElement>(databaseContext);
        var shopItemsRepository = new ShopItemsRepository(shopItemsSqlRepository, mapper);

        var shopStatsSqlRepository = new VersionedSqlRepository<ShopStatsStorageElement>(databaseContext);
        var shopStatsRepository = new ShopStatsRepository(shopStatsSqlRepository, mapper);

        UsersService = new UsersService(usersRepository);
        TransactionsService = new TransactionsService(transactionsRepository);
        EconomyService = new EconomyService(economiesRepository, TransactionsService);
        ItemsService = new ItemsService(
            new ItemsValidator(EconomyService, itemsRepository),
            itemsRepository,
            EconomyService
        );
        ShopsService = new ShopsService(
            shopsRepository,
            shopItemsRepository,
            shopStatsRepository,
            new ShopsValidator(EconomyService, shopItemsRepository),
            EconomyService,
            ItemsService,
            mapper
        );
        NewUserService = new NewUserService(usersRepository, EconomyService, ShopsService, mapper);
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
    protected IItemsService ItemsService { get; private set; } = null!;
    protected IShopsService ShopsService { get; private set; } = null!;
    protected IFixture Fixture { get; private set; } = null!;
    protected User User { get; private set; } = null!;
}