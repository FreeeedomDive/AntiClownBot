using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex;

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
        var currentActive = await antiClownEntertainmentApiClient.ActiveEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count: {count}", currentActive.Length);
        await antiClownEntertainmentApiClient.ActiveEventsIndex.ActualizeIndexAsync();
        currentActive = await antiClownEntertainmentApiClient.ActiveEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after actualizing: {count}", currentActive.Length);
        var allEvents = await antiClownEntertainmentApiClient.ActiveEventsIndex.ReadAllEventTypesAsync();
        foreach (var @event in allEvents)
        {
            await antiClownEntertainmentApiClient.ActiveEventsIndex.UpdateAsync(new ActiveCommonEventIndexDto
            {
                EventType = @event.Key,
                IsActive = true,
            });
        }

        currentActive = await antiClownEntertainmentApiClient.ActiveEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after enabling them all: {count}", currentActive.Length);
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}