using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;

public class F1PredictionsResultBuilder : IF1PredictionsResultBuilder
{
    public F1PredictionResult[] Build(F1Race race)
    {
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
        HashSet<string> teamMatesWinners = race.Season switch
        {
            2024 => Get2024TeamMatesWinners(driverToPosition),
            2025 => Get2025TeamMatesWinners(race, driverToPosition),
            _ => [],
        };
        var closestLeadDifference = race.Season == 2023
            ? 0
            : race.Predictions.Min(x => Math.Abs(race.Result.FirstPlaceLead - x.FirstPlaceLeadPrediction));

        return race.Predictions.Select(prediction => race.Season switch
        {
            2023 => Build2023Result(race, prediction, driverToPosition),
            2024 => Build2024Result(race, prediction, driverToPosition, teamMatesWinners, closestLeadDifference),
            2025 => Build2025Result(race, prediction, driverToPosition, teamMatesWinners, closestLeadDifference),
            _ => Build2026Result(race, prediction, driverToPosition, closestLeadDifference),
        }).ToArray();
    }

    private static F1PredictionResult Build2023Result(F1Race race, F1Prediction prediction, Dictionary<string, int> positions)
    {
        var result = new F1PredictionResult
        {
            RaceId = race.Id,
            UserId = prediction.UserId,
            TenthPlacePoints = GetTenthPlacePoints(prediction, positions),
            DnfsPoints = race.Result.DnfDrivers.Length > 0 &&
                         prediction.DnfPrediction.DnfPickedDrivers?.FirstOrDefault() == race.Result.DnfDrivers[0]
                ? 5
                : 0,
        };
        result.TotalPoints = result.TenthPlacePoints + result.DnfsPoints;
        return result;
    }

    private static F1PredictionResult Build2024Result(
        F1Race race,
        F1Prediction prediction,
        Dictionary<string, int> positions,
        HashSet<string> teamMatesWinners,
        decimal closestLeadDifference
    )
    {
        var result = new F1PredictionResult
        {
            RaceId = race.Id,
            UserId = prediction.UserId,
            TenthPlacePoints = GetTenthPlacePoints(prediction, positions),
            DnfsPoints = GetDnfPoints(race, prediction),
            SafetyCarsPoints = GetSafetyCarsPoints(race, prediction),
            FirstPlaceLeadPoints = GetFirstPlaceLeadPoints(race, prediction, closestLeadDifference),
            TeamMatesPoints = GetTeamMatesPoints(prediction, teamMatesWinners),
        };
        // 2024 sprints award the same points as full races.
        result.TotalPoints = result.TenthPlacePoints + result.DnfsPoints + result.SafetyCarsPoints +
                             result.FirstPlaceLeadPoints + result.TeamMatesPoints;
        return result;
    }

    private static F1PredictionResult Build2025Result(
        F1Race race,
        F1Prediction prediction,
        Dictionary<string, int> positions,
        HashSet<string> teamMatesWinners,
        decimal closestLeadDifference
    )
    {
        var result = new F1PredictionResult
        {
            RaceId = race.Id,
            UserId = prediction.UserId,
            TenthPlacePoints = GetTenthPlacePoints(prediction, positions),
            DnfsPoints = GetDnfPoints(race, prediction),
            SafetyCarsPoints = GetSafetyCarsPoints(race, prediction),
            FirstPlaceLeadPoints = GetFirstPlaceLeadPoints(race, prediction, closestLeadDifference),
            TeamMatesPoints = GetTeamMatesPoints(prediction, teamMatesWinners),
        };
        var fullPoints = result.TenthPlacePoints + result.DnfsPoints + result.SafetyCarsPoints +
                         result.FirstPlaceLeadPoints + result.TeamMatesPoints;
        result.TotalPoints = F1PredictionsHelper.CalculatePoints(fullPoints, 2025, race.IsSprint);
        return result;
    }

    private static F1PredictionResult Build2026Result(
        F1Race race,
        F1Prediction prediction,
        Dictionary<string, int> positions,
        decimal closestLeadDifference
    )
    {
        var result = new F1PredictionResult
        {
            RaceId = race.Id,
            UserId = prediction.UserId,
            TenthPlacePoints = GetTenthPlacePoints(prediction, positions),
            DnfsPoints = GetDnfPoints(race, prediction),
            SafetyCarsPoints = GetSafetyCarsPoints(race, prediction),
            FirstPlaceLeadPoints = GetFirstPlaceLeadPoints(race, prediction, closestLeadDifference),
            DriverPositionPoints = race.Conditions?.PositionPredictionDriver is null
                ? 0
                : F1PredictionsHelper.GetPositionPredictionPoints(
                    prediction.DriverPositionPrediction,
                    positions.GetValueOrDefault(race.Conditions.PositionPredictionDriver)
                ),
        };
        result.TotalPoints = result.TenthPlacePoints + result.DnfsPoints + result.SafetyCarsPoints +
                             result.FirstPlaceLeadPoints + result.DriverPositionPoints;
        return result;
    }

    private static int GetTenthPlacePoints(F1Prediction prediction, Dictionary<string, int> positions)
    {
        return F1PredictionsHelper.PointsByFinishPlaceDistribution.GetValueOrDefault(
            positions.GetValueOrDefault(prediction.TenthPlacePickedDriver)
        );
    }

    private static int GetDnfPoints(F1Race race, F1Prediction prediction)
    {
        if (prediction.DnfPrediction.NoDnfPredicted)
        {
            return race.Result.DnfDrivers.Length == 0 ? F1PredictionsHelper.NoDnfPredictionPoints : 0;
        }

        return (prediction.DnfPrediction.DnfPickedDrivers ?? [])
               .Intersect(race.Result.DnfDrivers).Count() * F1PredictionsHelper.DnfPredictionPoints;
    }

    private static int GetSafetyCarsPoints(F1Race race, F1Prediction prediction)
    {
        return prediction.SafetyCarsPrediction == ToSafetyCarsEnum(race.Result.SafetyCars)
            ? F1PredictionsHelper.IncidentsPredictionPoints
            : 0;
    }

    private static int GetFirstPlaceLeadPoints(F1Race race, F1Prediction prediction, decimal closestLeadDifference)
    {
        return Math.Abs(race.Result.FirstPlaceLead - prediction.FirstPlaceLeadPrediction) == closestLeadDifference ? 5 : 0;
    }

    private static int GetTeamMatesPoints(F1Prediction prediction, HashSet<string> teamMatesWinners)
    {
        return (prediction.TeamsPickedDrivers ?? []).Intersect(teamMatesWinners).Count();
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

    private static HashSet<string> Get2024TeamMatesWinners(Dictionary<string, int> positions)
    {
        // Historic teams must come from the race season, not the current teams repository.
        string PickPresent(params string[] candidates) => candidates.FirstOrDefault(positions.ContainsKey) ?? candidates[0];

        (string First, string Second)[] teams =
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

        return GetTeamMatesWinners(teams, positions);
    }

    private static HashSet<string> Get2025TeamMatesWinners(F1Race race, Dictionary<string, int> positions)
    {
        string PickPresent(params string[] candidates) => candidates.FirstOrDefault(positions.ContainsKey) ?? candidates[0];

        // Lawson and Tsunoda swapped teams after Australia and China in 2025.
        var beforeRedBullSwap = race.Name.StartsWith("Австралия", StringComparison.OrdinalIgnoreCase) ||
                                race.Name.StartsWith("Китай", StringComparison.OrdinalIgnoreCase);
        (string First, string Second)[] teams =
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

        return GetTeamMatesWinners(teams, positions);
    }

    private static HashSet<string> GetTeamMatesWinners((string First, string Second)[] teams, Dictionary<string, int> positions)
    {
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
