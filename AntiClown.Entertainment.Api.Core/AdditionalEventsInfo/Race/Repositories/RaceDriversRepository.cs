using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using AutoMapper;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;

public class RaceDriversRepository : IRaceDriversRepository
{
    public RaceDriversRepository(
        ISqlRepository<RaceDriverStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<RaceDriver[]> ReadAllAsync()
    {
        var result = await sqlRepository.ReadAllAsync();
        return mapper.Map<RaceDriver[]>(result);
    }

    public async Task<RaceDriver> FindAsync(string name)
    {
        return mapper.Map<RaceDriver>(await FindFirstAsync(name));
    }

    public async Task CreateAsync(RaceDriver raceDriver)
    {
        var storageElement = mapper.Map<RaceDriverStorageElement>(raceDriver);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(RaceDriver raceDriver)
    {
        var current = await FindFirstAsync(raceDriver.DriverName);
        await sqlRepository.UpdateAsync(current.Id, x =>
        {
            x.Points = raceDriver.Points;
            x.AccelerationSkill = raceDriver.AccelerationSkill;
            x.BreakingSkill = raceDriver.BreakingSkill;
            x.CorneringSkill = raceDriver.CorneringSkill;
        });
    }

    private async Task<RaceDriverStorageElement> FindFirstAsync(string name)
    {
        return (await sqlRepository.FindAsync(x => x.DriverName == name)).FirstOrDefault() ?? throw new DriverNotFoundException(name);
    }

    private readonly ISqlRepository<RaceDriverStorageElement> sqlRepository;
    private readonly IMapper mapper;
}