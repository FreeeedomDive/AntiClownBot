using AntiClown.Core.Dto.Exceptions;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Repositories.ActiveEventsIndex;

public class CommonActiveEventsIndexRepository : ICommonActiveEventsIndexRepository
{
    public CommonActiveEventsIndexRepository(ISqlRepository<ActiveEventsIndexStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<Dictionary<CommonEventType, bool>> ReadAllEventTypesAsync()
    {
        var result = new Dictionary<CommonEventType, bool>();
        var events = await sqlRepository.ReadAllAsync();
        foreach (var activeEventIndex in events)
        {
            if (!Enum.TryParse(activeEventIndex.EventType, out CommonEventType x))
            {
                continue;
            }

            result[x] = activeEventIndex.IsActive;
        }

        return result;
    }

    public async Task CreateAsync(CommonEventType eventType, bool isActiveByDefault)
    {
        if (await FindByTypeAsync(eventType) is not null)
        {
            throw new EntityAlreadyExistsException(eventType.ToString());
        }

        await sqlRepository.CreateAsync(new ActiveEventsIndexStorageElement
        {
            Id = Guid.NewGuid(),
            EventType = eventType.ToString(),
            IsActive = isActiveByDefault,
        });
    }

    public async Task UpdateAsync(CommonEventType eventType, bool isActive)
    {
        var existing = await FindByTypeAsync(eventType);
        if (existing is null)
        {
            throw new EntityNotFoundException(eventType.ToString());
        }

        await sqlRepository.UpdateAsync(existing.Id, x => x.IsActive = isActive);
    }

    private async Task<ActiveEventsIndexStorageElement?> FindByTypeAsync(CommonEventType eventType)
    {
        return (await sqlRepository.FindAsync(x => x.EventType == eventType.ToString())).FirstOrDefault();
    }

    private readonly ISqlRepository<ActiveEventsIndexStorageElement> sqlRepository;
}