using AntiClown.Api.Core.Shops.Domain;
using AntiClown.Api.Dto.Shops;
using AutoMapper;

namespace AntiClown.Api.Mappings;

public class ShopsDtoMapperProfile : Profile
{
    public ShopsDtoMapperProfile()
    {
        CreateMap<ShopItem, ShopItemDto>().ReverseMap();
        CreateMap<CurrentShopInfo, CurrentShopInfoDto>().ReverseMap();
        CreateMap<ShopStats, ShopStatsDto>();
    }
}