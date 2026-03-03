using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Extensions.Items;

public static class DogWifeDtoExtensions
{
    public static Dictionary<string, string> ItemDescription(this DogWifeDto dogWifeDto) => new()
    {
        {"Шанс получить лутбокс во время подношения", $"{(double)dogWifeDto.LootBoxFindChance / 10}%"},
    };
}