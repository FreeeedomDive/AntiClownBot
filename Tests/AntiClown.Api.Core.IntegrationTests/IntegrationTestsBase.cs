using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.IntegrationTests.Common;
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
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Settings;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using NSubstitute;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.IntegrationTests;

public class IntegrationTestsBase
{
    [OneTimeSetUp]
    public void SetUpDependencies()
    {
        // TODO: move to xUnit + TestContainers + WebApplicationFactory
        // Fixture = new Fixture();
        // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        // var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddMaps(assemblies));
        // var mapper = mapperConfiguration.CreateMapper();
        //
        // var connectionStringProvider = new TestsConnectionStringProvider();
        // var dbContextFactory = new DbContextFactory<DatabaseContext>();
        //
        // var usersSqlRepository = new SqlRepository<UserStorageElement>(dbContextFactory);
        // var usersRepository = new UsersRepository(usersSqlRepository, mapper);
        //
        // var usersSqlIntegrationsRepository = new SqlRepository<UserIntegrationStorageElement>(dbContextFactory);
        // var userIntegrationsRepository = new UserIntegrationsRepository(usersSqlIntegrationsRepository);
        //
        // var economiesSqlRepository = new VersionedSqlRepository<EconomyStorageElement>(dbContextFactory);
        // var economiesRepository = new EconomyRepository(economiesSqlRepository, mapper);
        //
        // var transactionsSqlRepository = new SqlRepository<TransactionStorageElement>(dbContextFactory);
        // var transactionsRepository = new TransactionsRepository(transactionsSqlRepository, mapper);
        //
        // var itemsSqlRepository = new SqlRepository<ItemStorageElement>(dbContextFactory);
        // var itemsRepository = new ItemsRepository(itemsSqlRepository, mapper);
        //
        // var shopsSqlRepository = new VersionedSqlRepository<ShopStorageElement>(dbContextFactory);
        // ShopsRepository = new ShopsRepository(shopsSqlRepository, mapper);
        //
        // var shopItemsSqlRepository = new SqlRepository<ShopItemStorageElement>(dbContextFactory);
        // ShopItemsRepository = new ShopItemsRepository(shopItemsSqlRepository, mapper);
        //
        // var shopStatsSqlRepository = new VersionedSqlRepository<ShopStatsStorageElement>(dbContextFactory);
        // ShopStatsRepository = new ShopStatsRepository(shopStatsSqlRepository, mapper);
        //
        // AntiClownDataApiClient = Substitute.For<IAntiClownDataApiClient>();
        // ConfigureDataApiClientMock();
        //
        // UsersService = new UsersService(usersRepository, userIntegrationsRepository);
        // TransactionsService = new TransactionsService(transactionsRepository);
        // EconomyService = new EconomyService(economiesRepository, UsersService, TransactionsService, AntiClownDataApiClient);
        // LohotronRewardGenerator = Substitute.For<ILohotronRewardGenerator>();
        // LohotronService = new LohotronService(EconomyService, LohotronRewardGenerator);
        // ItemsService = new ItemsService(
        //     new ItemsValidator(EconomyService, itemsRepository, AntiClownDataApiClient),
        //     itemsRepository,
        //     EconomyService,
        //     AntiClownDataApiClient
        // );
        // ShopsService = new ShopsService(
        //     ShopsRepository,
        //     ShopItemsRepository,
        //     ShopStatsRepository,
        //     new ShopsValidator(EconomyService, ShopItemsRepository, AntiClownDataApiClient),
        //     EconomyService,
        //     ItemsService,
        //     AntiClownDataApiClient,
        //     mapper
        // );
        // NewUserService = new NewUserService(usersRepository, EconomyService, ShopsService, mapper);
        // Scheduler = new SchedulerMock();
        // TributeService = new TributeService(EconomyService, ItemsService, new TributeMessageProducerMock(), AntiClownDataApiClient, Scheduler);
    }

    private void ConfigureDataApiClientMock()
    {
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Inventory.ToString(), "MaximumActiveItemsOfOneType").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Inventory.ToString(),
                    Name = "MaximumActiveItemsOfOneType",
                    Value = TestConstants.MaximumActiveItemsOfOneType.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Inventory.ToString(), "SellItemPercent").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Inventory.ToString(),
                    Name = "MaximumItemsInShop",
                    Value = TestConstants.SellItemPercent.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Economy.ToString(), "DefaultTributeCooldown").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Economy.ToString(),
                    Name = "DefaultTributeCooldown",
                    Value = TestConstants.DefaultCooldown.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Economy.ToString(), "DefaultScamCoins").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Economy.ToString(),
                    Name = "DefaultScamCoins",
                    Value = TestConstants.DefaultScamCoins.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Shop.ToString(), "RevealShopItemPercent").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Shop.ToString(),
                    Name = "RevealShopItemPercent",
                    Value = TestConstants.RevealShopItemPercent.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Shop.ToString(), "DefaultReRollPrice").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Shop.ToString(),
                    Name = "DefaultReRollPrice",
                    Value = TestConstants.DefaultReRollPrice.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Shop.ToString(), "DefaultReRollPriceIncrease").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Shop.ToString(),
                    Name = "DefaultReRollPriceIncrease",
                    Value = TestConstants.DefaultReRollPriceIncrease.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Shop.ToString(), "MaximumItemsInShop").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Shop.ToString(),
                    Name = "MaximumItemsInShop",
                    Value = TestConstants.MaximumItemsInShop.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(SettingsCategory.Shop.ToString(), "FreeItemRevealsPerDay").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = SettingsCategory.Shop.ToString(),
                    Name = "MaximumItemsInShop",
                    Value = TestConstants.FreeItemRevealsPerDay.ToString(),
                }
            )
        );
    }

    [SetUp]
    public async Task SetUp()
    {
        var newUserId = await NewUserService.CreateNewUserAsync(
            new NewUser
            {
                DiscordId = CreateUniqueUlong(),
            }
        );
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
    protected ILohotronRewardGenerator LohotronRewardGenerator { get; private set; } = null!;
    protected ILohotronService LohotronService { get; private set; } = null!;
    protected IAntiClownDataApiClient AntiClownDataApiClient { get; private set; } = null!;
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