using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Transfusion;
using AutoMapper;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;

public class CommonEventsRepository : ICommonEventsRepository
{
    public CommonEventsRepository(
        ISqlRepository<CommonEventStorageElement> sqlRepository,
        IMapper mapper
    )
    {
        this.sqlRepository = sqlRepository;
        this.mapper = mapper;
    }

    public async Task<CommonEventBase> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return Deserialize(result);
    }

    public async Task CreateAsync(CommonEventBase commonEvent)
    {
        var storageElement = mapper.Map<CommonEventStorageElement>(commonEvent);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task UpdateAsync(CommonEventBase commonEvent)
    {
        var storageElement = mapper.Map<CommonEventStorageElement>(commonEvent);
        await sqlRepository.UpdateAsync(commonEvent.Id, x =>
        {
            x.Finished = storageElement.Finished;
            x.Details = storageElement.Details;
        });
    }

    private static CommonEventBase Deserialize(CommonEventStorageElement storageElement)
    {
        var serialized = storageElement.Details;
        var eventType = Enum.TryParse<CommonEventType>(storageElement.Type, out var type)
            ? type
            : throw new InvalidOperationException($"Unexpected event type {storageElement.Type} in {nameof(CommonEventStorageElement)} {storageElement.Id}");
        return eventType switch
        {
            CommonEventType.GuessNumber => JsonConvert.DeserializeObject<GuessNumberEvent>(serialized)!,
            CommonEventType.Lottery => JsonConvert.DeserializeObject<LotteryEvent>(serialized)!,
            CommonEventType.Race => JsonConvert.DeserializeObject<RaceEvent>(serialized)!,
            CommonEventType.RemoveCoolDowns => JsonConvert.DeserializeObject<RemoveCoolDownsEvent>(serialized)!,
            CommonEventType.Transfusion => JsonConvert.DeserializeObject<TransfusionEvent>(serialized)!,
            _ => throw new ArgumentOutOfRangeException(nameof(eventType))
        };
    }

    private readonly ISqlRepository<CommonEventStorageElement> sqlRepository;
    private readonly IMapper mapper;
}