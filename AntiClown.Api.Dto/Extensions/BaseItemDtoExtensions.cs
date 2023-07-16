using AntiClown.Api.Dto.Inventories;

namespace AntiClown.Api.Dto.Extensions;

public static class BaseItemDtoExtensions
{
    public static IEnumerable<CatWifeDto> CatWives(this IEnumerable<BaseItemDto> items)
    {
        return items
               .Where(item => item.ItemName == ItemNameDto.CatWife)
               .Select(item => (item as CatWifeDto)!);
    }

    public static IEnumerable<CommunismBannerDto> CommunismBanners(this IEnumerable<BaseItemDto> items)
    {
        return items
               .Where(item => item.ItemName == ItemNameDto.CommunismBanner)
               .Select(item => (item as CommunismBannerDto)!);
    }

    public static IEnumerable<DogWifeDto> DogWives(this IEnumerable<BaseItemDto> items)
    {
        return items
               .Where(item => item.ItemName == ItemNameDto.DogWife)
               .Select(item => (item as DogWifeDto)!);
    }

    public static IEnumerable<InternetDto> Internets(this IEnumerable<BaseItemDto> items)
    {
        return items
               .Where(item => item.ItemName == ItemNameDto.Internet)
               .Select(item => (item as InternetDto)!);
    }

    public static IEnumerable<JadeRodDto> JadeRods(this IEnumerable<BaseItemDto> items)
    {
        return items
               .Where(item => item.ItemName == ItemNameDto.JadeRod)
               .Select(item => (item as JadeRodDto)!);
    }

    public static IEnumerable<RiceBowlDto> RiceBowls(this IEnumerable<BaseItemDto> items)
    {
        return items
               .Where(item => item.ItemName == ItemNameDto.RiceBowl)
               .Select(item => (item as RiceBowlDto)!);
    }
}