using AntiClown.Entertainment.Api.Client;

namespace AntiClown.Tools.Tools.EventsTests;

public class ActualizeCommonEventsIndexTool : ToolBase
{
    public ActualizeCommonEventsIndexTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<ActualizeCommonEventsIndexTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var currentActive = await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count: {count}", currentActive.Length);
        await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ActualizeIndexAsync();
        currentActive = await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after actualizing: {count}", currentActive.Length);
        var allEvents = await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ReadAllEventTypesAsync();
        foreach (var @event in allEvents)
        {
            await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.UpdateAsync(@event.Key, true);
        }

        currentActive = await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after enabling them all: {count}", currentActive.Length);
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}