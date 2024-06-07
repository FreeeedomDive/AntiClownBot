using AntiClown.Core.Dto.Exceptions;
using Xdd.HttpHelpers.Models.Exceptions;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Repositories.ActiveEventsIndex;

public class ActiveDailyEventsIndexRepository : IActiveDailyEventsIndexRepository
{
    public ActiveDailyEventsIndexRepository(ISqlRepository<DailyActiveEventsIndexStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<Dictionary<DailyEventType, bool>> ReadAllEventTypesAsync()
    {
        var result = new Dictionary<DailyEventType, bool>();
        var events = await sqlRepository.ReadAllAsync();
        foreach (var activeEventIndex in events)
        {
            if (!Enum.TryParse(activeEventIndex.EventType, out DailyEventType x))
            {
                continue;
            }

            result[x] = activeEventIndex.IsActive;
        }

        return result;
    }

    public async Task CreateAsync(DailyEventType eventType, bool isActiveByDefault)
    {
        if (await FindByTypeAsync(eventType) is not null)
        {
            throw new EntityAlreadyExistsException(eventType.ToString());
        }

        await sqlRepository.CreateAsync(
            new DailyActiveEventsIndexStorageElement
            {
                Id = Guid.NewGuid(),
                EventType = eventType.ToString(),
                IsActive = isActiveByDefault,
            }
        );
    }

    public async Task UpdateAsync(DailyEventType eventType, bool isActive)
    {
        var existing = await FindByTypeAsync(eventType);
        if (existing is null)
        {
            throw new EntityNotFoundException(eventType.ToString());
        }

        await sqlRepository.UpdateAsync(existing.Id, x => x.IsActive = isActive);
    }

    private async Task<DailyActiveEventsIndexStorageElement?> FindByTypeAsync(DailyEventType eventType)
    {
        return (await sqlRepository.FindAsync(x => x.EventType == eventType.ToString())).FirstOrDefault();
    }

    private readonly ISqlRepository<DailyActiveEventsIndexStorageElement> sqlRepository;
}