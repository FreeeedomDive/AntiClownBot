using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
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
        await sqlRepository.UpdateAsync(
            commonEvent.Id, x =>
            {
                x.Finished = storageElement.Finished;
                x.Details = storageElement.Details;
            }
        );
    }

    private static CommonEventBase Deserialize(CommonEventStorageElement storageElement)
    {
        var serialized = storageElement.Details;
        return JsonConvert.DeserializeObject<CommonEventBase>(
            serialized, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }
        )!;
    }

    private readonly IMapper mapper;

    private readonly ISqlRepository<CommonEventStorageElement> sqlRepository;
}