using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex;

namespace AntiClown.Tools.Tools.EventsTests;

public class ActualizeDailyEventsIndexTool : ToolBase
{
    public ActualizeDailyEventsIndexTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<ActualizeDailyEventsIndexTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var currentActive = await antiClownEntertainmentApiClient.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count: {count}", currentActive.Length);
        await antiClownEntertainmentApiClient.ActiveDailyEventsIndex.ActualizeIndexAsync();
        currentActive = await antiClownEntertainmentApiClient.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after actualizing: {count}", currentActive.Length);
        var allEvents = await antiClownEntertainmentApiClient.ActiveDailyEventsIndex.ReadAllEventTypesAsync();
        foreach (var @event in allEvents)
        {
            await antiClownEntertainmentApiClient.ActiveDailyEventsIndex.UpdateAsync(new ActiveDailyEventIndexDto
            {
                EventType = @event.Key, 
                IsActive = true,
            });
        }

        currentActive = await antiClownEntertainmentApiClient.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after enabling them all: {count}", currentActive.Length);
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}