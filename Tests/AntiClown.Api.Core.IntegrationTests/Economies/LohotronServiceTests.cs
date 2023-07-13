using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Dto.Exceptions.Economy;
using AutoFixture;
using FluentAssertions;

namespace AntiClown.Api.Core.IntegrationTests.Economies;

public class LohotronServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task LohotronService_Should_ThrowIfLohotronIsCalledSecondTime()
    {
        var economy1 = await EconomyService.ReadEconomyAsync(User.Id);
        economy1.IsLohotronReady.Should().BeTrue();

        var nothingReward = new LohotronReward
        {
            RewardType = LohotronRewardType.Nothing,
        };
        LohotronRewardGeneratorMock.Setup(x => x.Generate()).Returns(nothingReward);

        await LohotronService.UseLohotronAsync(User.Id);
        var economy2 = await EconomyService.ReadEconomyAsync(User.Id);
        economy2.IsLohotronReady.Should().BeFalse();

        Func<Task> action = () => LohotronService.UseLohotronAsync(User.Id);
        await action.Should().ThrowAsync<LohotronAlreadyUsedException>();

        var economy3 = await EconomyService.ReadEconomyAsync(User.Id);
        economy3.IsLohotronReady.Should().BeFalse();
    }

    [Test]
    public async Task LohotronService_Should_DoNothing_WithEmptyReward()
    {
        var nothingReward = new LohotronReward
        {
            RewardType = LohotronRewardType.Nothing,
        };
        LohotronRewardGeneratorMock.Setup(x => x.Generate()).Returns(nothingReward);
        var economyBefore = await EconomyService.ReadEconomyAsync(User.Id);

        var actualReward = await LohotronService.UseLohotronAsync(User.Id);
        actualReward.Should().BeEquivalentTo(nothingReward);

        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.ScamCoins.Should().Be(economyBefore.ScamCoins);
        economyAfter.LootBoxes.Should().Be(economyBefore.LootBoxes);
    }

    [TestCase(true), TestCase(false)]
    public async Task LohotronService_Should_GiveScamCoins_WithScamCoinsReward(bool isPositiveReward)
    {
        var scamCoinsReward = new LohotronReward
        {
            RewardType = LohotronRewardType.ScamCoins,
            ScamCoinsReward = Fixture.Create<int>() * (isPositiveReward ? 1 : -1),
        };
        LohotronRewardGeneratorMock.Setup(x => x.Generate()).Returns(scamCoinsReward);
        var economyBefore = await EconomyService.ReadEconomyAsync(User.Id);

        var actualReward = await LohotronService.UseLohotronAsync(User.Id);
        actualReward.Should().BeEquivalentTo(scamCoinsReward);

        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.ScamCoins.Should().Be(economyBefore.ScamCoins + scamCoinsReward.ScamCoinsReward);
    }

    [Test]
    public async Task LohotronService_Should_GiveLootBox_WithLootBoxReward()
    {
        var lootBoxReward = new LohotronReward
        {
            RewardType = LohotronRewardType.LootBox,
        };
        LohotronRewardGeneratorMock.Setup(x => x.Generate()).Returns(lootBoxReward);
        var economyBefore = await EconomyService.ReadEconomyAsync(User.Id);

        var actualReward = await LohotronService.UseLohotronAsync(User.Id);
        actualReward.Should().BeEquivalentTo(lootBoxReward);

        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.LootBoxes.Should().Be(economyBefore.LootBoxes + 1);
    }
}