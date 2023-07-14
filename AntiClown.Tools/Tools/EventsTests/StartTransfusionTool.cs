using AntiClown.Entertainment.Api.Client;
using Newtonsoft.Json;

namespace AntiClown.Tools.Tools.EventsTests;

public class StartTransfusionTool : ToolBase
{
    public StartTransfusionTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<StartTransfusionTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var eventId = await antiClownEntertainmentApiClient.CommonEvents.Transfusion.StartNewAsync();
        var transfusionEvent = await antiClownEntertainmentApiClient.CommonEvents.Transfusion.ReadAsync(eventId);
        Logger.LogInformation(JsonConvert.SerializeObject(transfusionEvent, Formatting.Indented));
    }

    public override string Name => nameof(StartTransfusionTool);
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}