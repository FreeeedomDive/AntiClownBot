using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public static class F1PredictionsResultBuilder
{
    public static F1PredictionResult[] Build(F1Race race)
    {
        if (!race.Predictions.Any())
        {
            return Array.Empty<F1PredictionResult>();
        }

        var position = 1;
        var driverToPosition = race.Result.Classification.ToDictionary(
            x => x, _ =>
            {
                var pos = position++;
                return pos;
            }
        );
        var teamMatesWinners = new[]
        {
            SelectHighestTeamMate(driverToPosition, F1Driver.Verstappen, F1Driver.Perez),
            SelectHighestTeamMate(driverToPosition, F1Driver.Leclerc, F1Driver.Sainz),
            SelectHighestTeamMate(driverToPosition, F1Driver.Hamilton, F1Driver.Russell),
            SelectHighestTeamMate(driverToPosition, F1Driver.Ocon, F1Driver.Gasly),
            SelectHighestTeamMate(driverToPosition, F1Driver.Piastri, F1Driver.Norris),
            SelectHighestTeamMate(driverToPosition, F1Driver.Bottas, F1Driver.Zhou),
            SelectHighestTeamMate(driverToPosition, F1Driver.Stroll, F1Driver.Alonso),
            SelectHighestTeamMate(driverToPosition, F1Driver.Magnussen, F1Driver.Hulkenberg),
            SelectHighestTeamMate(driverToPosition, F1Driver.Ricciardo, F1Driver.Tsunoda),
            SelectHighestTeamMate(driverToPosition, F1Driver.Albon, F1Driver.Sargeant),
        };

        var results = race
                      .Predictions
                      .Select(
                          prediction => new F1PredictionResult
                          {
                              RaceId = race.Id,
                              UserId = prediction.UserId,
                              TenthPlacePoints = F1PredictionsPointsHelper.PointsDistribution.GetValueOrDefault(
                                  driverToPosition.GetValueOrDefault(prediction.TenthPlacePickedDriver, 0), 0
                              ),
                              DnfsPoints = race.Result.DnfDrivers.Length == 0 && prediction.DnfPrediction.NoDnfPredicted
                                  ? 10
                                  : prediction.DnfPrediction.NoDnfPredicted
                                      ? 0
                                      : prediction.DnfPrediction.DnfPickedDrivers!.Intersect(race.Result.DnfDrivers).Count() * 2,
                              SafetyCarsPoints = prediction.SafetyCarsPrediction == ToSafetyCarsEnum(race.Result.SafetyCars) ? 5 : 0,
                              TeamMatesPoints = prediction.TeamsPickedDrivers.Intersect(teamMatesWinners).Count(),
                          }
                      )
                      .ToDictionary(x => x.UserId);

        var predictionsOrderedByFirstPlaceLeadDifference = race
                                                           .Predictions
                                                           .OrderBy(x => Math.Abs(race.Result.FirstPlaceLead - x.FirstPlaceLeadPrediction));
        var closestPrediction = predictionsOrderedByFirstPlaceLeadDifference.First();
        var closestFirstPlaceLeadPredictionResult = results[closestPrediction.UserId];
        closestFirstPlaceLeadPredictionResult.FirstPlaceLeadPoints = 5;

        return results.Values.ToArray();
    }

    public static F1SafetyCars ToSafetyCarsEnum(int safetyCarsCount)
    {
        return safetyCarsCount switch
        {
            0 => F1SafetyCars.Zero,
            1 => F1SafetyCars.One,
            2 => F1SafetyCars.Two,
            >= 3 => F1SafetyCars.ThreePlus,
            _ => throw new ArgumentOutOfRangeException(nameof(safetyCarsCount), safetyCarsCount, null),
        };
    }

    private static F1Driver SelectHighestTeamMate(Dictionary<F1Driver, int> positions, F1Driver firstDriver, F1Driver secondDriver)
    {
        return positions.GetValueOrDefault(firstDriver, 999) < positions.GetValueOrDefault(secondDriver, 999)
            ? firstDriver
            : secondDriver;
    }
}