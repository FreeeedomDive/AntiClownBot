using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AutoMapper;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;

public class DailyEventsRepository : IDailyEventsRepository
{
    public DailyEventsRepository(
        ISqlRepository<DailyEventStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<DailyEventBase> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return Deserialize(result);
    }

    public async Task CreateAsync(DailyEventBase commonEvent)
    {
        var storageElement = mapper.Map<DailyEventStorageElement>(commonEvent);
        await sqlRepository.CreateAsync(storageElement);
    }

    private static DailyEventBase Deserialize(DailyEventStorageElement storageElement)
    {
        var serialized = storageElement.Details;
        return JsonConvert.DeserializeObject<DailyEventBase>(
            serialized, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }
        )!;
    }

    private readonly IMapper mapper;

    private readonly ISqlRepository<DailyEventStorageElement> sqlRepository;
}