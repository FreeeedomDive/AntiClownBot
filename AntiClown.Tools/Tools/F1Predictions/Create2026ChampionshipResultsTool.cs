using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions;

namespace AntiClown.Tools.Tools.F1Predictions;

public class Create2026ChampionshipResultsTool(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    ILogger<Create2026ChampionshipResultsTool> logger
) : ToolBase(logger)
{
    protected override async Task RunAsync()
    {
        var teams = await antiClownEntertainmentApiClient.F1Predictions.ReadTeamsAsync();
        var standings = teams.SelectMany(x => new[] { x.FirstDriver, x.SecondDriver }).ToArray();

        await antiClownEntertainmentApiClient.F1ChampionshipPredictions.WriteResultsAsync(
            2026,
            new F1ChampionshipResultsDto
            {
                HasData = true,
                Stage = F1ChampionshipPredictionTypeDto.PreSeason,
                IsOpen = true,
                Standings = standings,
            }
        );
    }
}
