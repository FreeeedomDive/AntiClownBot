using AntiClown.Entertainment.Api.Client;

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
        var currentActive = await antiClownEntertainmentApiClient.DailyEvents.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count: {count}", currentActive.Length);
        await antiClownEntertainmentApiClient.DailyEvents.ActiveDailyEventsIndex.ActualizeIndexAsync();
        currentActive = await antiClownEntertainmentApiClient.DailyEvents.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after actualizing: {count}", currentActive.Length);
        var allEvents = await antiClownEntertainmentApiClient.DailyEvents.ActiveDailyEventsIndex.ReadAllEventTypesAsync();
        foreach (var @event in allEvents)
        {
            await antiClownEntertainmentApiClient.DailyEvents.ActiveDailyEventsIndex.UpdateAsync(@event.Key, true);
        }

        currentActive = await antiClownEntertainmentApiClient.DailyEvents.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after enabling them all: {count}", currentActive.Length);
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}