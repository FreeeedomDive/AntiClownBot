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
        await antiClownEntertainmentApiClient.CommonEvents.Bedge.StartNewAsync();
    }

    public override string Name => nameof(StartBedgeTool);
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}