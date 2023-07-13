using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain.Announce;
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
        var eventType = Enum.TryParse<DailyEventType>(storageElement.Type, out var type)
            ? type
            : throw new InvalidOperationException($"Unexpected event type {storageElement.Type} in {nameof(DailyEventStorageElement)} {storageElement.Id}");
        return eventType switch
        {
            DailyEventType.Announce => JsonConvert.DeserializeObject<AnnounceEvent>(serialized)!,
            DailyEventType.PaymentsAndResets => throw new NotSupportedException($"Storing of {nameof(DailyEventType.PaymentsAndResets)} events is not supported"),
            DailyEventType.CleanUpParties => throw new NotSupportedException($"Storing of {nameof(DailyEventType.CleanUpParties)} events is not supported"),
            _ => throw new ArgumentOutOfRangeException(nameof(eventType))
        };
    }

    private readonly ISqlRepository<DailyEventStorageElement> sqlRepository;
    private readonly IMapper mapper;
}