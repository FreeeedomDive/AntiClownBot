using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Core.Mappings;

public class RaceModelsMapperProfile : Profile
{
    public RaceModelsMapperProfile()
    {
        CreateMap<RaceTrackStorageElement, RaceTrack>();
        CreateMap<RaceTrack, RaceTrackStorageElement>()
            .ForMember(
                storageElement => storageElement.Id,
                cfg => cfg.MapFrom(_ => Guid.NewGuid())
            );
        CreateMap<RaceDriverStorageElement, RaceDriver>();
        CreateMap<RaceDriver, RaceDriverStorageElement>()
            .ForMember(
                storageElement => storageElement.Id,
                cfg => cfg.Ignore()
            );
    }
}