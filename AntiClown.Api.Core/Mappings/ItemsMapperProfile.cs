using AntiClown.Api.Core.Inventory.Domain.Items.Base;
using AntiClown.Api.Core.Inventory.Repositories;
using AutoMapper;
using Newtonsoft.Json;

namespace AntiClown.Api.Core.Mappings;

public class ItemsMapperProfile : Profile
{
    public ItemsMapperProfile()
    {
        CreateMap<BaseItem, ItemStorageElement>()
            .ForMember(
                se => se.Name,
                cfg => cfg.MapFrom(item => item.ItemName.ToString())
            )
            .ForMember(
                se => se.ItemSpecs,
                cfg => cfg.MapFrom(item => JsonConvert.SerializeObject(item, Formatting.Indented))
            );
    }
}