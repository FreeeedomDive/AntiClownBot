using AntiClown.EntertainmentApi.Client;

namespace AntiClown.Tools.Tools.EventsTests;

public class ActualizeEventsIndexTool : ToolBase
{
    public ActualizeEventsIndexTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<ActualizeEventsIndexTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var currentActive = await antiClownEntertainmentApiClient.CommonEvents.ActiveEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count: {count}", currentActive.Length);
        await antiClownEntertainmentApiClient.CommonEvents.ActiveEventsIndex.ActualizeIndexAsync();
        currentActive = await antiClownEntertainmentApiClient.CommonEvents.ActiveEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after actualizing: {count}", currentActive.Length);
        var allEvents = await antiClownEntertainmentApiClient.CommonEvents.ActiveEventsIndex.ReadAllEventTypesAsync();
        foreach (var @event in allEvents)
        {
            await antiClownEntertainmentApiClient.CommonEvents.ActiveEventsIndex.UpdateAsync(@event.Key, true);
        }
        currentActive = await antiClownEntertainmentApiClient.CommonEvents.ActiveEventsIndex.ReadActiveEventsAsync();
        Logger.LogInformation("Current active events count after enabling them all: {count}", currentActive.Length);
    }

    public override string Name => nameof(ActualizeEventsIndexTool);
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}