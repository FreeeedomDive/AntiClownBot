using AntiClown.Api.Dto.Exceptions.Tribute;
using FluentAssertions;

namespace AntiClown.Api.Core.IntegrationTests.Economies;

public class TributeServiceTests : IntegrationTestsBase
{
    [Test]
    public async Task TributeService_Should_MakeCorrectTributes()
    {
        var economyBefore = await EconomyService.ReadEconomyAsync(User.Id);
        var now = DateTime.Now;
        economyBefore.NextTribute.Should().BeBefore(now);
        var tribute = await TributeService.MakeTributeAsync(User.Id);
        var economyAfter = await EconomyService.ReadEconomyAsync(User.Id);
        economyAfter.NextTribute.Should().BeAfter(now);
        economyAfter.ScamCoins.Should().Be(economyBefore.ScamCoins + tribute.ScamCoins);
    }

    [Test]
    public async Task TributeService_Should_ThrowWhenTributeIsOnCooldown()
    {
        var makeTributeAction = () => TributeService.MakeTributeAsync(User.Id);
        await makeTributeAction.Should().NotThrowAsync();
        await makeTributeAction.Should().ThrowAsync<TributeIsOnCooldownException>();
    }
}