using AntiClown.Api.Core.Shops.Domain;
using FluentAssertions;
using SqlRepositoryBase.Core.Exceptions;

namespace IntegrationTests.Shops;

public class ShopsServiceTests : IntegrationTestsBase
{
    [SetUp]
    public async Task EconomySetUp()
    {
        try
        {
            shop = await ShopsService.ReadCurrentShopAsync(User.Id);
        }
        catch (SqlEntityNotFoundException)
        {
            Assert.Fail($"{nameof(NewUserService)} did not create economy for user {User.Id}");
        }
    }

    [Test]
    public void NewUserService_Should_CreateNewShopWithItems()
    {
        var @default = Shop.Default;
        shop.Id.Should().Be(User.Id);
        shop.ReRollPrice.Should().Be(@default.ReRollPrice);
        shop.FreeReveals.Should().Be(@default.FreeReveals);
        shop.Items.Should().NotBeEmpty();
    }

    private CurrentShopInfo shop = null!;
}