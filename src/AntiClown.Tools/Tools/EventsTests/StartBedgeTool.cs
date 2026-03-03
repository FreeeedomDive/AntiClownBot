using AntiClown.Entertainment.Api.Client;

namespace AntiClown.Tools.Tools.EventsTests;

public class StartBedgeTool : ToolBase
{
    public StartBedgeTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<StartBedgeTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownEntertainmentApiClient.BedgeEvent.StartNewAsync();
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}