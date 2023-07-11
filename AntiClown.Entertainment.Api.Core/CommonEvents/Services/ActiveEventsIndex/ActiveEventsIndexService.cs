using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories.ActiveEventsIndex;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.ActiveEventsIndex;

public class ActiveEventsIndexService : IActiveEventsIndexService
{
    public ActiveEventsIndexService(IActiveEventsIndexRepository activeEventsIndexRepository)
    {
        this.activeEventsIndexRepository = activeEventsIndexRepository;
    }

    public async Task<Dictionary<CommonEventType, bool>> ReadAllEventTypesAsync()
    {
        return await activeEventsIndexRepository.ReadAllEventTypesAsync();
    }

    public async Task<CommonEventType[]> ReadActiveEventsAsync()
    {
        var events = await ReadAllEventTypesAsync();
        return events.Where(kv => kv.Value).Select(kv => kv.Key).ToArray();
    }

    public async Task CreateAsync(CommonEventType eventType, bool isActiveByDefault)
    {
        await activeEventsIndexRepository.CreateAsync(eventType, isActiveByDefault);
    }

    public async Task UpdateAsync(CommonEventType eventType, bool isActive)
    {
        await activeEventsIndexRepository.UpdateAsync(eventType, isActive);
    }

    public async Task ActualizeIndexAsync()
    {
        var currentIndexState = await ReadAllEventTypesAsync();
        var eventTypes = Enum.GetValues<CommonEventType>();
        var eventsWithoutIndex = eventTypes.Where(x => !currentIndexState.TryGetValue(x, out _)).ToArray();
        foreach (var eventType in eventsWithoutIndex)
        {
            await CreateAsync(eventType, false);
        }
    }

    private readonly IActiveEventsIndexRepository activeEventsIndexRepository;
}