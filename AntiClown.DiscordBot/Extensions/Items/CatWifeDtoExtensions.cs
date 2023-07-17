using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Extensions.Items;

public static class CatWifeDtoExtensions
{
    public static Dictionary<string, string> ItemDescription(this CatWifeDto catWifeDto) => new()
    {
        {"Шанс на автоматическое подношение", $"{catWifeDto.AutoTributeChance}%"},
    };
}