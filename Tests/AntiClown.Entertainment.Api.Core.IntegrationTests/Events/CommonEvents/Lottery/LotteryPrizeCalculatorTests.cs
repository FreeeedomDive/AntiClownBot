using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;
using AntiClown.Tools.Utility.Extensions;
using FluentAssertions;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.Events.CommonEvents.Lottery;

public class LotteryPrizeCalculatorTests
{
    [Test]
    public void NoSlots_Should_ReturnZero()
    {
        LotteryPrizeCalculator.Calculate(Array.Empty<LotterySlot>()).Should().Be(0);
    }

    [Test]
    public void FiveAndSevenSameSlots_Should_GiveSamePrize()
    {
        var slot = Enum.GetValues<LotterySlot>().SelectRandomItem();
        var result1 = LotteryPrizeCalculator.Calculate(Enumerable.Range(0, 5).Select(_ => slot));
        var result2 = LotteryPrizeCalculator.Calculate(Enumerable.Range(0, 7).Select(_ => slot));
        var result3 = LotteryPrizeCalculator.Calculate(Enumerable.Range(0, 69).Select(_ => slot));
        result1.Should().Be(result2);
        result2.Should().Be(result3);
    }

    // random combinations
    [TestCase(new[] { LotterySlot.Tier1, LotterySlot.Tier1, LotterySlot.Tier1, LotterySlot.Tier2, LotterySlot.Tier2, LotterySlot.Tier2, LotterySlot.Tier3 }, 60),
     TestCase(new[] { LotterySlot.Tier7, LotterySlot.Tier7, LotterySlot.Tier7, LotterySlot.Tier4, LotterySlot.Tier4, LotterySlot.Tier4, LotterySlot.Tier3 }, 460),
     TestCase(new[] { LotterySlot.Tier10, LotterySlot.Tier10, LotterySlot.Tier10, LotterySlot.Tier10, LotterySlot.Tier10, LotterySlot.Tier9, LotterySlot.Tier9 }, 5200),
     TestCase(new[] { LotterySlot.Tier2, LotterySlot.Tier1, LotterySlot.Tier9, LotterySlot.Tier4, LotterySlot.Tier5, LotterySlot.Tier10, LotterySlot.Tier3 }, 140),
     TestCase(new[] { LotterySlot.Tier7, LotterySlot.Tier6, LotterySlot.Tier4, LotterySlot.Tier4, LotterySlot.Tier7, LotterySlot.Tier8, LotterySlot.Tier3 }, 295),
     TestCase(new[] { LotterySlot.Tier8, LotterySlot.Tier2, LotterySlot.Tier4, LotterySlot.Tier1, LotterySlot.Tier5, LotterySlot.Tier5, LotterySlot.Tier3 }, 165),
     TestCase(new[] { LotterySlot.Tier1, LotterySlot.Tier3, LotterySlot.Tier9, LotterySlot.Tier6, LotterySlot.Tier1, LotterySlot.Tier4, LotterySlot.Tier6 }, 190),
     TestCase(new[] { LotterySlot.Tier7, LotterySlot.Tier8, LotterySlot.Tier5, LotterySlot.Tier5, LotterySlot.Tier5, LotterySlot.Tier9, LotterySlot.Tier9 }, 465),
     TestCase(new[] { LotterySlot.Tier6, LotterySlot.Tier6, LotterySlot.Tier9, LotterySlot.Tier3, LotterySlot.Tier7, LotterySlot.Tier2, LotterySlot.Tier2 }, 230),
     TestCase(new[] { LotterySlot.Tier7, LotterySlot.Tier4, LotterySlot.Tier4, LotterySlot.Tier1, LotterySlot.Tier3, LotterySlot.Tier3, LotterySlot.Tier5 }, 175)]
    // real cases from last lotteries
    public void Cases(LotterySlot[] slots, int prize)
    {
        LotteryPrizeCalculator.Calculate(slots).Should().Be(prize);
    }
}