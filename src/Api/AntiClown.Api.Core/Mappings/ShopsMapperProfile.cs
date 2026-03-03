using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Shops.Domain;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Core.Shops.Repositories.Shops;
using AntiClown.Api.Core.Shops.Repositories.Stats;
using AutoMapper;

namespace AntiClown.Api.Core.Mappings;

public class ShopsMapperProfile : Profile
{
    public ShopsMapperProfile()
    {
        CreateMap<Shop, ShopStorageElement>().ReverseMap();
        CreateMap<ShopItem, ShopItemStorageElement>().ReverseMap();
        CreateMap<ShopStats, ShopStatsStorageElement>().ReverseMap();
        CreateMap<BaseItem, ShopItem>()
            .ForMember(
                shopItem => shopItem.ShopId,
                cfg => cfg.MapFrom(item => item.OwnerId)
            )
            .ForMember(
                shopItem => shopItem.Name,
                cfg => cfg.MapFrom(item => item.ItemName)
            )
            .ForMember(
                shopItem => shopItem.IsRevealed,
                cfg => cfg.MapFrom(_ => false)
            )
            .ForMember(
                shopItem => shopItem.IsOwned,
                cfg => cfg.MapFrom(_ => false)
            );
    }
}