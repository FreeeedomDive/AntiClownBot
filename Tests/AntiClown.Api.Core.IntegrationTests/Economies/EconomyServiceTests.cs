﻿using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.IntegrationTests.Common;
using AntiClown.Tools.Utility.Extensions;
using AutoFixture;
using FluentAssertions;
using SqlRepositoryBase.Core.Exceptions;

namespace AntiClown.Api.Core.IntegrationTests.Economies;

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
        economy.Should().BeEquivalentTo(new Economy
        {
            Id = User.Id,
            LootBoxes = 0,
            IsLohotronReady = true,
            ScamCoins = TestConstants.DefaultScamCoins,
        }, options => options.Excluding(x => x.NextTribute));
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

    [Test]
    public async Task EconomyService_Should_ResetAllCoolDowns()
    {
        const int economiesCount = 5;
        var ids = Enumerable.Range(1, economiesCount).Select(_ => Guid.NewGuid()).ToArray();
        ids.ForEach(id => EconomyService.CreateEmptyAsync(id).GetAwaiter().GetResult());
        var economies = ids.Select(id => EconomyService.ReadEconomyAsync(id).GetAwaiter().GetResult()).ToArray();
        var resetMoment = DateTime.UtcNow;
        economies.Select(x => x.NextTribute).Select(x => x < resetMoment).Should().AllBeEquivalentTo(true);
        await EconomyService.ResetAllCoolDownsAsync();
        economies = ids.Select(id => EconomyService.ReadEconomyAsync(id).GetAwaiter().GetResult()).ToArray();
        economies.Select(x => x.NextTribute).Select(x => x > resetMoment).Should().AllBeEquivalentTo(true);
    }

    private Economy economy = null!;
}