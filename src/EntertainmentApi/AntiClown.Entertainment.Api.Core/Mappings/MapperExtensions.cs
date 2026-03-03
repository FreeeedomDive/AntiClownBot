using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Core.Mappings;

public static class MapperExtensions
{
    public static RaceDriverStorageElement Map(this IMapper mapper, RaceDriver raceDriver, Guid id)
    {
        var storageElement = mapper.Map<RaceDriverStorageElement>(raceDriver);
        storageElement.Id = id;
        return storageElement;
    }
}