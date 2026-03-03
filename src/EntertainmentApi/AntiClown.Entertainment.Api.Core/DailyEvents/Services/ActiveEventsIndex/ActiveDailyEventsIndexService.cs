using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories.ActiveEventsIndex;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services.ActiveEventsIndex;

public class ActiveDailyEventsIndexService : IActiveDailyEventsIndexService
{
    public ActiveDailyEventsIndexService(IActiveDailyEventsIndexRepository activeDailyEventsIndexRepository)
    {
        this.activeDailyEventsIndexRepository = activeDailyEventsIndexRepository;
    }

    public async Task<Dictionary<DailyEventType, bool>> ReadAllEventTypesAsync()
    {
        return await activeDailyEventsIndexRepository.ReadAllEventTypesAsync();
    }

    public async Task<DailyEventType[]> ReadActiveEventsAsync()
    {
        var events = await ReadAllEventTypesAsync();
        return events.Where(kv => kv.Value).Select(kv => kv.Key).ToArray();
    }

    public async Task CreateAsync(DailyEventType eventType, bool isActiveByDefault)
    {
        await activeDailyEventsIndexRepository.CreateAsync(eventType, isActiveByDefault);
    }

    public async Task UpdateAsync(DailyEventType eventType, bool isActive)
    {
        await activeDailyEventsIndexRepository.UpdateAsync(eventType, isActive);
    }

    public async Task ActualizeIndexAsync()
    {
        var currentIndexState = await ReadAllEventTypesAsync();
        var eventTypes = Enum.GetValues<DailyEventType>();
        var eventsWithoutIndex = eventTypes.Where(x => !currentIndexState.TryGetValue(x, out _)).ToArray();
        foreach (var eventType in eventsWithoutIndex)
        {
            await CreateAsync(eventType, false);
        }
    }

    private readonly IActiveDailyEventsIndexRepository activeDailyEventsIndexRepository;
}