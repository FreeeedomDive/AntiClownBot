using AntiClown.Api.Core.Inventory.Domain;
using AntiClown.Api.Core.Inventory.Domain.Items;
using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Dto.Inventories;
using AutoMapper;

namespace AntiClown.Api.Mappings;

public class InventoriesDtoMapperProfile : Profile
{
    public InventoriesDtoMapperProfile()
    {
        CreateMap<ItemName, ItemNameDto>();
        CreateMap<Rarity, RarityDto>();
        CreateMap<ItemType, ItemTypeDto>();
        CreateMap<ItemsFilterDto, ItemsFilter>();
        CreateMap<BaseItem, BaseItemDto>();
        CreateMap<CatWife, CatWifeDto>();
        CreateMap<CommunismBanner, CommunismBannerDto>();
        CreateMap<DogWife, DogWifeDto>();
        CreateMap<Internet, InternetDto>();
        CreateMap<JadeRod, JadeRodDto>();
        CreateMap<RiceBowl, RiceBowlDto>();
    }
}