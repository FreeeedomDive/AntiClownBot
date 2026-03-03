using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;

public static class F1ChampionshipPointsBuilder
{
    public static F1ChampionshipUserPoints[] Build(
        F1ChampionshipResults results,
        F1ChampionshipPrediction[] predictions
    )
    {
        return predictions.Select(p => BuildForUser(results, p)).ToArray();
    }

    private static F1ChampionshipUserPoints BuildForUser(
        F1ChampionshipResults results,
        F1ChampionshipPrediction prediction
    )
    {
        var countPreSeason = ShouldCountPoints(results, F1ChampionshipPredictionType.PreSeason);
        var countMidSeason = ShouldCountPoints(results, F1ChampionshipPredictionType.MidSeason);

        var driverToActualPosition = results.HasData && results.Standings.Length > 0
            ? results.Standings.Select((d, i) => (d, i)).ToDictionary(x => x.d, x => x.i)
            : new Dictionary<string, int>();

        return new F1ChampionshipUserPoints
        {
            UserId = prediction.UserId,
            PreSeasonPoints = CalculatePoints(prediction.PreSeasonStandings, driverToActualPosition, PreSeasonPointsTable, countPreSeason),
            MidSeasonPoints = CalculatePoints(prediction.MidSeasonStandings, driverToActualPosition, MidSeasonPointsTable, countMidSeason),
        };
    }

    private static int[] CalculatePoints(
        string[]? predicted,
        Dictionary<string, int> driverToActualPosition,
        int[] pointsTable,
        bool countPoints
    )
    {
        if (predicted is null || predicted.Length == 0)
        {
            return [];
        }

        if (!countPoints || driverToActualPosition.Count == 0)
        {
            return new int[predicted.Length];
        }

        return predicted.Select((driver, predictedPosition) =>
            {
                if (!driverToActualPosition.TryGetValue(driver, out var actualPosition))
                {
                    return 0;
                }

                var diff = Math.Abs(predictedPosition - actualPosition);
                return diff < pointsTable.Length ? pointsTable[diff] : 0;
            }
        ).ToArray();
    }

    private static bool ShouldCountPoints(F1ChampionshipResults results, F1ChampionshipPredictionType type)
    {
        if (!results.HasData)
        {
            return false;
        }

        return type switch
        {
            F1ChampionshipPredictionType.PreSeason => results is { Stage: F1ChampionshipPredictionType.PreSeason, IsOpen: false }
                                                      || results.Stage == F1ChampionshipPredictionType.MidSeason,
            F1ChampionshipPredictionType.MidSeason => results is { Stage: F1ChampionshipPredictionType.MidSeason, IsOpen: false },
            _ => false,
        };
    }

    private static readonly int[] PreSeasonPointsTable = [20, 14, 11, 8, 5, 2];
    private static readonly int[] MidSeasonPointsTable = [10, 6, 4, 2, 1];
}