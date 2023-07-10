namespace AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;

public static class LotteryPrizeCalculator
{
    public static int Calculate(IEnumerable<LotterySlot> slots)
    {
        return slots
            .GroupBy(x => x)
            .Select(group => SlotToPrize(group.Key) * SameSlotsCountToMultiplier(group.Count()))
            .Sum();
    }

    private static int SlotToPrize(LotterySlot slot)
    {
        return slot switch
        {
            LotterySlot.Tier1 => 0,
            LotterySlot.Tier2 => 5,
            LotterySlot.Tier3 => 10,
            LotterySlot.Tier4 => 15,
            LotterySlot.Tier5 => 20,
            LotterySlot.Tier6 => 25,
            LotterySlot.Tier7 => 30,
            LotterySlot.Tier8 => 35,
            LotterySlot.Tier9 => 40,
            LotterySlot.Tier10 => 50,
            _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
        };
    }

    private static int SameSlotsCountToMultiplier(int count)
    {
        return count switch
        {
            1 => 1,
            2 => 5,
            3 => 10,
            4 => 50,
            >= 5 => 100,
            _ => throw new ArgumentOutOfRangeException(nameof(count), count, null)
        };
    }
}