using AntiClown.Api.Core.Economies.Domain;
using AutoFixture;
using FluentAssertions;
using SqlRepositoryBase.Core.Exceptions;

namespace IntegrationTests.Economies;

public class EconomyServiceTests : IntegrationTestsBase
{
    [SetUp]
    public async Task EconomySetUp()
    {
        try
        {
            economy = await EconomyService.ReadEconomyAsync(User.Id);
        }
        catch (SqlEntityNotFoundException)
        {
            Assert.Fail($"{nameof(NewUserService)} did not create economy for user {User.Id}");
        }
    }
    
    [Test]
    public void NewUserService_Should_CreateNewEconomy()
    {
        // создание объекта экономики провалидирует EconomySetUp, а тест проверит, что созданный объект валидный
        economy.Should().BeEquivalentTo(Economy.Default);
    }

    [Test]
    public async Task EconomyService_Should_UpdateScamCoins()
    {
        var balanceBefore = economy.ScamCoins;
        var diff = Fixture.Create<int>();
        await EconomyService.UpdateScamCoinsAsync(User.Id, diff, "Тест");
        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.ScamCoins.Should().Be(balanceBefore + diff);
    }

    [Test]
    public async Task EconomyService_Should_UpdateLootBoxes()
    {
        var balanceBefore = economy.LootBoxes;
        var diff = Fixture.Create<int>() % 10;
        await EconomyService.UpdateLootBoxesAsync(User.Id, diff);
        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.LootBoxes.Should().Be(balanceBefore + diff);
    }

    private Economy economy = null!;
}