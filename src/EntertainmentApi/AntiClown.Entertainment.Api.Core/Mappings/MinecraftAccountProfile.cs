using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;
using AntiClown.Entertainment.Api.Core.MinecraftAuth.Repositories;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Core.Mappings;

public class MinecraftAccountProfile : Profile
{
    public MinecraftAccountProfile()
    {
        CreateMap<MinecraftAccount, MinecraftAccountStorageElement>()
            .ForMember(x => x.Id,
                cfg => cfg.MapFrom(e => e.Id))
            .ReverseMap();
    }
}