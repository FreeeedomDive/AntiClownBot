using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Messages.Dto.Tributes;
using AutoMapper;

namespace AntiClown.Api.Core.Mappings;

public class EconomiesMapperProfile: Profile
{
    public EconomiesMapperProfile()
    {
        CreateMap<Economy, EconomyStorageElement>().ReverseMap();
        CreateMap<Tribute, TributeMessageDto>();
    }
}