using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Tools.Tools.F1Predictions;

public class Create2026GridTool(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    ILogger<Create2026GridTool> logger
) : ToolBase(logger)
{
    protected override async Task RunAsync()
    {
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("01 McLaren", "Norris", "Piastri");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("02 Mercedes", "Russell", "Antonelli");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("03 Red Bull Racing", "Verstappen", "Hadjar");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("04 Ferrari", "Leclerc", "Hamilton");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("05 Williams", "Sainz", "Albon");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("06 Racing Bulls", "Lawson", "Lindblad");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("07 Aston Martin", "Alonso", "Stroll");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("08 Haas", "Ocon", "Bearman");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("09 Audi", "Hulkenberg", "Bortoleto");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("10 Alpine", "Gasly", "Colapinto");
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync("11 Cadillac", "Perez", "Bottas");
    }
}