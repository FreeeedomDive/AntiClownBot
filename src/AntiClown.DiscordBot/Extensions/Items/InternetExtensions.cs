using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Extensions.Items;

public static class InternetExtensions
{
    public static Dictionary<string, string> ItemDescription(this InternetDto internetDto) => new()
    {
        {"Шанс уменьшения кулдауна во время одной попытки", $"{internetDto.Speed}%"},
        {"Попытки уменьшить кулдаун", $"{internetDto.Gigabytes}"},
        {"Уменьшение кулдауна в процентах", $"{internetDto.Ping}%"},
    };
}