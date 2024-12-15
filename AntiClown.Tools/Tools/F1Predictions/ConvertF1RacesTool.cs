using AntiClown.Entertainment.Api.Client;

namespace AntiClown.Tools.Tools.F1Predictions;

public class ConvertF1RacesTool : ToolBase
{
    public ConvertF1RacesTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<ConvertF1RacesTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownEntertainmentApiClient.F1Predictions.ConvertAsync();
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}