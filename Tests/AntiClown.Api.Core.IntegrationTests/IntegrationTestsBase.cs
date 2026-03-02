using AntiClown.Api.Core.Achievements.Services;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.IntegrationTests.Common;
using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Core.Shops.Repositories.Shops;
using AntiClown.Api.Core.Shops.Repositories.Stats;
using AntiClown.Api.Core.Shops.Services;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Services;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Tests.Configuration;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AntiClown.Api.Core.IntegrationTests;

public abstract class IntegrationTestsBase<TFactory>
    : IntegrationTestsBase<TFactory, Program>
    where TFactory : IntegrationTestsWebApplicationFactory<Program>, new()
{
    protected IAchievementsService AchievementsService { get; private set; } = null!;
    protected IUsersService UsersService { get; private set; } = null!;
    protected INewUserService NewUserService { get; private set; } = null!;
    protected ITransactionsService TransactionsService { get; private set; } = null!;
    protected IEconomyService EconomyService { get; private set; } = null!;
    protected ILohotronService LohotronService { get; private set; } = null!;
    protected IItemsService ItemsService { get; private set; } = null!;
    protected IShopsService ShopsService { get; private set; } = null!;
    protected ITributeService TributeService { get; private set; } = null!;
    protected IShopsRepository ShopsRepository { get; private set; } = null!;
    protected IShopItemsRepository ShopItemsRepository { get; private set; } = null!;
    protected IShopStatsRepository ShopStatsRepository { get; private set; } = null!;
    protected IAntiClownDataApiClient AntiClownDataApiClient { get; private set; } = null!;
    protected IFixture Fixture { get; private set; } = null!;
    protected User User { get; private set; } = null!;

    [OneTimeSetUp]
    public void InitializeServices()
    {
        Fixture = new Fixture();

        AchievementsService = Scope.ServiceProvider.GetRequiredService<IAchievementsService>();
        UsersService = Scope.ServiceProvider.GetRequiredService<IUsersService>();
        NewUserService = Scope.ServiceProvider.GetRequiredService<INewUserService>();
        TransactionsService = Scope.ServiceProvider.GetRequiredService<ITransactionsService>();
        EconomyService = Scope.ServiceProvider.GetRequiredService<IEconomyService>();
        LohotronService = Scope.ServiceProvider.GetRequiredService<ILohotronService>();
        ItemsService = Scope.ServiceProvider.GetRequiredService<IItemsService>();
        ShopsService = Scope.ServiceProvider.GetRequiredService<IShopsService>();
        TributeService = Scope.ServiceProvider.GetRequiredService<ITributeService>();
        ShopsRepository = Scope.ServiceProvider.GetRequiredService<IShopsRepository>();
        ShopItemsRepository = Scope.ServiceProvider.GetRequiredService<IShopItemsRepository>();
        ShopStatsRepository = Scope.ServiceProvider.GetRequiredService<IShopStatsRepository>();
        AntiClownDataApiClient = Scope.ServiceProvider.GetRequiredService<IAntiClownDataApiClient>();

        ConfigureDataApiClientMock();
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

    private void ConfigureDataApiClientMock()
    {
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Inventory), "MaximumActiveItemsOfOneType").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Inventory),
                    Name = "MaximumActiveItemsOfOneType",
                    Value = TestConstants.MaximumActiveItemsOfOneType.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Inventory), "SellItemPercent").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Inventory),
                    Name = "MaximumItemsInShop",
                    Value = TestConstants.SellItemPercent.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Economy), "DefaultTributeCooldown").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Economy),
                    Name = "DefaultTributeCooldown",
                    Value = TestConstants.DefaultCooldown.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Economy), "DefaultScamCoins").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Economy),
                    Name = "DefaultScamCoins",
                    Value = TestConstants.DefaultScamCoins.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Shop), "RevealShopItemPercent").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Shop),
                    Name = "RevealShopItemPercent",
                    Value = TestConstants.RevealShopItemPercent.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Shop), "DefaultReRollPrice").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Shop),
                    Name = "DefaultReRollPrice",
                    Value = TestConstants.DefaultReRollPrice.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Shop), "DefaultReRollPriceIncrease").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Shop),
                    Name = "DefaultReRollPriceIncrease",
                    Value = TestConstants.DefaultReRollPriceIncrease.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Shop), "MaximumItemsInShop").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Shop),
                    Name = "MaximumItemsInShop",
                    Value = TestConstants.MaximumItemsInShop.ToString(),
                }
            )
        );
        AntiClownDataApiClient.Settings.ReadAsync(nameof(SettingsCategory.Shop), "FreeItemRevealsPerDay").Returns(
            Task.FromResult(
                new SettingDto
                {
                    Category = nameof(SettingsCategory.Shop),
                    Name = "MaximumItemsInShop",
                    Value = TestConstants.FreeItemRevealsPerDay.ToString(),
                }
            )
        );
    }
}

public abstract class IntegrationTestsBase
    : IntegrationTestsBase<ApiIntegrationTestsWebApplicationFactory>
{
}
