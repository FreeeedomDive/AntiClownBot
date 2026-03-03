using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Tools.Tools.F1Predictions;

public class Calculate2024SeasonWithNewSprintPointsTool : ToolBase
{
    public Calculate2024SeasonWithNewSprintPointsTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        
        ILogger<Calculate2024SeasonWithNewSprintPointsTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var seasonRaces = (await antiClownEntertainmentApiClient.F1Predictions.FindAsync(
            new F1RaceFilterDto
            {
                Season = 2024,
            }
        )).ToDictionary(x => x.Id);
        var standings = await antiClownEntertainmentApiClient.F1Predictions.ReadStandingsAsync(2024);
        var predictionsTable = standings
            .Select(
                kv =>
                {
                    var oldPoints = kv.Value.Select(x => x?.TotalPoints ?? 0).ToArray();
                    var newPoints = kv.Value.Select(
                        x =>
                            x is null
                                ? 0
                                : seasonRaces[x.RaceId].IsSprint
                                    ? x.TotalPoints * 30 / 100
                                    : x.TotalPoints
                    ).ToArray();
                    return new
                    {
                        UserId = kv.Key,
                        PredictionsOld = oldPoints,
                        PredictionsNew = newPoints,
                        TotalPointsOld = oldPoints.Sum(),
                        TotalPointsNew = newPoints.Sum(),
                    };
                }
            ).ToArray();
        foreach (var userPredictions in predictionsTable)
        {
            Console.WriteLine($"{userPredictions.UserId}: Old={userPredictions.TotalPointsOld}, New={userPredictions.TotalPointsNew}");
            Console.WriteLine($"Old: {string.Join(" ", userPredictions.PredictionsOld)}");
            Console.WriteLine($"New: {string.Join(" ", userPredictions.PredictionsNew)}");
            Console.WriteLine();
        }
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}