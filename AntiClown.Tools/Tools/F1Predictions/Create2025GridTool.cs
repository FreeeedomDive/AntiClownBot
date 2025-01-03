using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Tools.Tools.F1Predictions;

public class Create2025GridTool : ToolBase
{
    public Create2025GridTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<Create2025GridTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("McLaren", "Norris", "Piastri");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Ferrari", "Leclerc", "Hamilton");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Red Bull Racing", "Verstappen", "Lawson");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Mercedes", "Russell", "Antonelli");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Aston Martin", "Alonso", "Stroll");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Alpine", "Gasly", "Doohan");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Haas", "Ocon", "Bearman");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("RB", "Tsunoda", "Hadjar");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Williams", "Sainz", "Albon");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("Stake", "Hulkenberg", "Bortoleto");
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}