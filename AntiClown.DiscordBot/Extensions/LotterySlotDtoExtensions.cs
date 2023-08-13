using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;

namespace AntiClown.DiscordBot.Extensions;

public static class LotterySlotDtoExtensions
{
    public static string ToEmoteName(this LotterySlotDto lotterySlot)
    {
        return lotterySlot switch
        {
            LotterySlotDto.Tier1 => "Starege",
            LotterySlotDto.Tier2 => "KEKWiggle",
            LotterySlotDto.Tier3 => "FLOPPA",
            LotterySlotDto.Tier4 => "Applecatrun",
            LotterySlotDto.Tier5 => "cykaPls",
            LotterySlotDto.Tier6 => "PaPaTuTuWaWa",
            LotterySlotDto.Tier7 => "PolarStrut",
            LotterySlotDto.Tier8 => "HACKERMANS",
            LotterySlotDto.Tier9 => "PATREGO",
            LotterySlotDto.Tier10 => "triangD",
            _ => throw new ArgumentOutOfRangeException(nameof(lotterySlot), lotterySlot, null),
        };
    }

    public static int ToPrize(this LotterySlotDto lotterySlot)
    {
        return lotterySlot switch
        {
            LotterySlotDto.Tier1 => 0,
            LotterySlotDto.Tier2 => 5,
            LotterySlotDto.Tier3 => 10,
            LotterySlotDto.Tier4 => 15,
            LotterySlotDto.Tier5 => 20,
            LotterySlotDto.Tier6 => 25,
            LotterySlotDto.Tier7 => 30,
            LotterySlotDto.Tier8 => 35,
            LotterySlotDto.Tier9 => 40,
            LotterySlotDto.Tier10 => 50,
            _ => throw new ArgumentOutOfRangeException(nameof(lotterySlot), lotterySlot, null),
        };
    }
}