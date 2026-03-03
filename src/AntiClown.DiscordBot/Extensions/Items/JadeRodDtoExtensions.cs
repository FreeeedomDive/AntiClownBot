using AntiClown.Api.Dto.Economies;
using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Extensions.Items;

public static class JadeRodDtoExtensions
{
    public static Dictionary<string, string> ItemDescription(this JadeRodDto jadeRodDto) => new()
    {
        {
            "Шанс увеличения кулдауна во время одной попытки", $"{TributeHelpers.CooldownIncreaseChanceByOneJade}%"
        },
        {
            "Попытки увеличить кулдаун", $"{jadeRodDto.Length}"
        },
        {
            "Увеличение кулдауна в процентах", $"{jadeRodDto.Thickness}%"
        },
    };
}