using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AutoMapper;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;

public class RaceTracksRepository : IRaceTracksRepository
{
    public RaceTracksRepository(
        ISqlRepository<RaceTrackStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<RaceTrack[]> ReadAllAsync()
    {
        var result = await sqlRepository.ReadAllAsync();
        return mapper.Map<RaceTrack[]>(result);
    }

    public async Task CreateAsync(RaceTrack raceTrack)
    {
        var storageElement = mapper.Map<RaceTrackStorageElement>(raceTrack);
        await sqlRepository.CreateAsync(storageElement);
    }

    private readonly ISqlRepository<RaceTrackStorageElement> sqlRepository;
    private readonly IMapper mapper;
}