using AntiClown.Api.Dto.Inventories;

namespace AntiClown.DiscordBot.Extensions.Items;

public static class CommunismBannerExtensions
{
    public static Dictionary<string, string> ItemDescription(this CommunismBannerDto communismBannerDto) => new()
    {
        {
            "Шанс разделить награду за подношение с другим владельцем плаката",
            $"{communismBannerDto.DivideChance}%"
        },
        {
            "Приоритет стащить чужое подношение (если у него сработал плакат)",
            $"{communismBannerDto.StealChance}"
        },
    };
}