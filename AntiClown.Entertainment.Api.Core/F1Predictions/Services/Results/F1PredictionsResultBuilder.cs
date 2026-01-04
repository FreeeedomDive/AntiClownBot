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

        var resultsByUserId = race
                              .Predictions
                              .Select(prediction => new F1PredictionResult
                                  {
                                      RaceId = race.Id,
                                      UserId = prediction.UserId,
                                      TenthPlacePoints = F1PredictionsHelper.PointsByFinishPlaceDistribution.GetValueOrDefault(
                                          driverToPosition.GetValueOrDefault(prediction.TenthPlacePickedDriver)
                                      ),
                                      DnfsPoints = race.Result.DnfDrivers.Length == 0 && prediction.DnfPrediction.NoDnfPredicted
                                          ? F1PredictionsHelper.NoDnfPredictionPoints
                                          : prediction.DnfPrediction.NoDnfPredicted
                                              ? 0
                                              : prediction.DnfPrediction.DnfPickedDrivers!.Intersect(race.Result.DnfDrivers).Count()
                                                * F1PredictionsHelper.DnfPredictionPoints,
                                      SafetyCarsPoints = prediction.SafetyCarsPrediction == ToSafetyCarsEnum(race.Result.SafetyCars)
                                          ? F1PredictionsHelper.IncidentsPredictionPoints
                                          : 0,
                                      DriverPositionPoints = F1PredictionsHelper.GetPositionPredictionPoints(
                                          prediction.DriverPositionPrediction,
                                          driverToPosition.GetValueOrDefault(race.Conditions.PositionPredictionDriver)
                                      ),
                                  }
                              )
                              .ToDictionary(x => x.UserId);

        var predictionsOrderedByFirstPlaceLeadDifference = race
                                                           .Predictions
                                                           .OrderBy(x => Math.Abs(race.Result.FirstPlaceLead - x.FirstPlaceLeadPrediction));
        var closestPrediction = predictionsOrderedByFirstPlaceLeadDifference.First();
        var closestFirstPlaceLeadPredictionResult = resultsByUserId[closestPrediction.UserId];
        closestFirstPlaceLeadPredictionResult.FirstPlaceLeadPoints = 5;

        var results = resultsByUserId.Values.Select(x =>
            {
                x.TotalPoints = x.TenthPlacePoints + x.DnfsPoints + x.SafetyCarsPoints + x.FirstPlaceLeadPoints + x.DriverPositionPoints;
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
}