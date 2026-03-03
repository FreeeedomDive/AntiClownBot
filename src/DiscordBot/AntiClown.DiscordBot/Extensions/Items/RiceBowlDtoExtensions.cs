using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Extensions.Items;

public static class RiceBowlDtoExtensions
{
    public static Dictionary<string, string> ItemDescription(this RiceBowlDto riceBowlDto) => new()
    {
        {
            "Расширение границ полученной награды с подношения",
            $"в отрицательную сторону - {riceBowlDto.NegativeRangeExtend}, " +
            $"в положительную сторону - {riceBowlDto.PositiveRangeExtend}"
        },
    };
}