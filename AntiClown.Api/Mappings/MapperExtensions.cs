using AntiClown.Api.Core.Inventory.Domain.Items;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Domain.Items.Base.Extensions;
using AntiClown.Api.Dto.Inventories;
using AutoMapper;

namespace AntiClown.Api.Mappings;

public static class MapperExtensions
{
    public static BaseItemDto Map(this IMapper mapper, BaseItem item)
    {
        return item.ItemName switch
        {
            ItemName.CatWife => mapper.Map<CatWifeDto>(item as CatWife),
            ItemName.CommunismBanner => mapper.Map<CommunismBannerDto>(item as CommunismBanner),
            ItemName.DogWife => mapper.Map<DogWifeDto>(item as DogWife),
            ItemName.Internet => mapper.Map<InternetDto>(item as Internet),
            ItemName.JadeRod => mapper.Map<JadeRodDto>(item as JadeRod),
            ItemName.RiceBowl => mapper.Map<RiceBowlDto>(item as RiceBowl),
            _ => throw new ArgumentOutOfRangeException(nameof(item.ItemName)),
        };
    }

    public static InventoryDto MapInventory(this IMapper mapper, BaseItem[] items)
    {
        return new InventoryDto
        {
            CatWives = items.CatWives().Select(x => (mapper.Map(x) as CatWifeDto)!).ToArray(),
            CommunismBanners = items.CommunismBanners().Select(x => (mapper.Map(x) as CommunismBannerDto)!).ToArray(),
            DogWives = items.DogWives().Select(x => (mapper.Map(x) as DogWifeDto)!).ToArray(),
            Internets = items.Internets().Select(x => (mapper.Map(x) as InternetDto)!).ToArray(),
            JadeRods = items.JadeRods().Select(x => (mapper.Map(x) as JadeRodDto)!).ToArray(),
            RiceBowls = items.RiceBowls().Select(x => (mapper.Map(x) as RiceBowlDto)!).ToArray(),
            NetWorth = items.Where(x => x.ItemType != ItemType.Negative).Sum(x => x.Price),
        };
    }
}