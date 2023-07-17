using AntiClown.Api.Dto.Inventories;
using AntiClown.DiscordBot.Extensions.Items;

namespace AntiClown.DiscordBot.Extensions;

public static class BaseItemDtoExtensions
{
    public static Dictionary<string, string> Description(this BaseItemDto baseItemDto)
    {
        return baseItemDto.ItemName switch
        {
            ItemNameDto.CatWife => (baseItemDto as CatWifeDto)!.ItemDescription(),
            ItemNameDto.CommunismBanner => (baseItemDto as CommunismBannerDto)!.ItemDescription(),
            ItemNameDto.DogWife => (baseItemDto as DogWifeDto)!.ItemDescription(),
            ItemNameDto.Internet => (baseItemDto as InternetDto)!.ItemDescription(),
            ItemNameDto.JadeRod => (baseItemDto as JadeRodDto)!.ItemDescription(),
            ItemNameDto.RiceBowl => (baseItemDto as RiceBowlDto)!.ItemDescription(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}