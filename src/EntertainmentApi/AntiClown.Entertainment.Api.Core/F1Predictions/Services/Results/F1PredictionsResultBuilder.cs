using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;

public class F1PredictionsResultBuilder : IF1PredictionsResultBuilder
{
    public F1PredictionResult[] Build(F1Race race)
    {
        if (race.Season is < 2023 or > 2026)
        {
            throw new NotSupportedException($"F1 prediction rules for season {race.Season} are not defined");
        }

        if (race.Predictions.Count == 0)
        {
            return [];
        }

        var position = 1;
        var driverToPosition = race.Result.Classification.ToDictionary(
            x => x, _ =>
            {
                var pos = position++;
                return pos;
            }
        );
        var teamMatesWinners = race.Season is 2024 or 2025
            ? GetTeamMatesWinners(race, driverToPosition)
            : [];

        var resultsByUserId = race
                              .Predictions
                              .Select(prediction => new F1PredictionResult
                                  {
                                      RaceId = race.Id,
                                      UserId = prediction.UserId,
                                      TenthPlacePoints = F1PredictionsHelper.PointsByFinishPlaceDistribution.GetValueOrDefault(
                                          driverToPosition.GetValueOrDefault(prediction.TenthPlacePickedDriver)
                                      ),
                                      DnfsPoints = race.Season == 2023
                                          ? race.Result.DnfDrivers.Length > 0 &&
                                            prediction.DnfPrediction.DnfPickedDrivers?.FirstOrDefault() == race.Result.DnfDrivers[0]
                                              ? 5
                                              : 0
                                          : race.Result.DnfDrivers.Length == 0 && prediction.DnfPrediction.NoDnfPredicted
                                              ? F1PredictionsHelper.NoDnfPredictionPoints
                                              : prediction.DnfPrediction.NoDnfPredicted
                                                  ? 0
                                                  : (prediction.DnfPrediction.DnfPickedDrivers ?? [])
                                                    .Intersect(race.Result.DnfDrivers).Count()
                                                    * F1PredictionsHelper.DnfPredictionPoints,
                                      SafetyCarsPoints = race.Season >= 2024 &&
                                                        prediction.SafetyCarsPrediction == ToSafetyCarsEnum(race.Result.SafetyCars)
                                          ? F1PredictionsHelper.IncidentsPredictionPoints
                                          : 0,
                                      TeamMatesPoints = race.Season is 2024 or 2025
                                          ? (prediction.TeamsPickedDrivers ?? []).Intersect(teamMatesWinners).Count()
                                          : 0,
                                      DriverPositionPoints = race.Season != 2026 || race.Conditions?.PositionPredictionDriver is null
                                          ? 0
                                          : F1PredictionsHelper.GetPositionPredictionPoints(
                                              prediction.DriverPositionPrediction,
                                              driverToPosition.GetValueOrDefault(race.Conditions.PositionPredictionDriver)
                                          ),
                                  }
                              )
                              .ToDictionary(x => x.UserId);

        if (race.Season >= 2024)
        {
            var closestDifference = race.Predictions.Min(x => Math.Abs(race.Result.FirstPlaceLead - x.FirstPlaceLeadPrediction));
            foreach (var prediction in race.Predictions.Where(x => Math.Abs(race.Result.FirstPlaceLead - x.FirstPlaceLeadPrediction) == closestDifference))
            {
                resultsByUserId[prediction.UserId].FirstPlaceLeadPoints = 5;
            }
        }

        var results = resultsByUserId.Values.Select(x =>
            {
                x.TotalPoints = x.TenthPlacePoints + x.DnfsPoints + x.SafetyCarsPoints + x.FirstPlaceLeadPoints +
                                x.TeamMatesPoints + x.DriverPositionPoints;
                x.TotalPoints = F1PredictionsHelper.CalculatePoints(x.TotalPoints, race.Season, race.IsSprint);

                return x;
            }
        ).ToArray();

        return results;
    }

    public static SafetyCarsCount ToSafetyCarsEnum(int safetyCarsCount)
    {
        return safetyCarsCount switch
        {
            0 => SafetyCarsCount.Zero,
            1 => SafetyCarsCount.One,
            2 => SafetyCarsCount.Two,
            >= 3 => SafetyCarsCount.ThreePlus,
            _ => throw new ArgumentOutOfRangeException(nameof(safetyCarsCount), safetyCarsCount, null),
        };
    }

    private static HashSet<string> GetTeamMatesWinners(F1Race race, Dictionary<string, int> positions)
    {
        // Historic teams must come from the race season, not the current teams repository.
        string PickPresent(params string[] candidates) => candidates.FirstOrDefault(positions.ContainsKey) ?? candidates[0];

        (string First, string Second)[] teams;
        if (race.Season == 2024)
        {
            teams =
            [
                ("Verstappen", "Perez"),
                ("Leclerc", PickPresent("Sainz", "Bearman")),
                ("Hamilton", "Russell"),
                ("Gasly", PickPresent("Ocon", "Doohan")),
                ("Norris", "Piastri"),
                ("Bottas", "Zhou"),
                ("Alonso", "Stroll"),
                ("Hulkenberg", PickPresent("Magnussen", "Bearman")),
                ("Tsunoda", PickPresent("Ricciardo", "Lawson")),
                ("Albon", PickPresent("Sargeant", "Colapinto")),
            ];
        }
        else
        {
            // Lawson and Tsunoda swapped teams after Australia and China in 2025.
            var beforeRedBullSwap = race.Name.StartsWith("Австралия", StringComparison.OrdinalIgnoreCase) ||
                                    race.Name.StartsWith("Китай", StringComparison.OrdinalIgnoreCase);
            teams =
            [
                ("Verstappen", beforeRedBullSwap ? "Lawson" : "Tsunoda"),
                ("Leclerc", "Hamilton"),
                ("Russell", "Antonelli"),
                ("Gasly", PickPresent("Doohan", "Colapinto")),
                ("Norris", "Piastri"),
                ("Hulkenberg", "Bortoleto"),
                ("Alonso", "Stroll"),
                ("Ocon", "Bearman"),
                ("Hadjar", beforeRedBullSwap ? "Tsunoda" : "Lawson"),
                ("Albon", "Sainz"),
            ];
        }

        return teams
               .Select(team => SelectHigherTeamMate(positions, team.First, team.Second))
               .Where(winner => winner is not null)
               .Select(winner => winner!)
               .ToHashSet();
    }

    private static string? SelectHigherTeamMate(Dictionary<string, int> positions, string firstDriver, string secondDriver)
    {
        var firstPosition = positions.GetValueOrDefault(firstDriver, int.MaxValue);
        var secondPosition = positions.GetValueOrDefault(secondDriver, int.MaxValue);
        if (firstPosition == int.MaxValue && secondPosition == int.MaxValue)
        {
            return null;
        }

        return firstPosition < secondPosition ? firstDriver : secondDriver;
    }
}
