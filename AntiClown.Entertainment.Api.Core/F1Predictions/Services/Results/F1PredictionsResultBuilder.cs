using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Teams;
using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;

public class F1PredictionsResultBuilder(IF1PredictionTeamsRepository f1PredictionTeamsRepository) : IF1PredictionsResultBuilder
{
    public async Task<F1PredictionResult[]> Build(F1Race race)
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
        var teams = await f1PredictionTeamsRepository.ReadAllAsync();
        var teamMatesWinners = teams
                               .Select(x => SelectHighestTeamMate(driverToPosition, x.FirstDriver, x.SecondDriver))
                               .ToArray();

        var resultsByUserId = race
                              .Predictions
                              .Select(prediction => new F1PredictionResult
                                  {
                                      RaceId = race.Id,
                                      UserId = prediction.UserId,
                                      TenthPlacePoints = F1PredictionsPointsHelper.PointsByFinishPlaceDistribution.GetValueOrDefault(
                                          driverToPosition.GetValueOrDefault(prediction.TenthPlacePickedDriver, 0), 0
                                      ),
                                      DnfsPoints = race.Result.DnfDrivers.Length == 0 && prediction.DnfPrediction.NoDnfPredicted
                                          ? F1PredictionsPointsHelper.NoDnfPredictionPoints
                                          : prediction.DnfPrediction.NoDnfPredicted
                                              ? 0
                                              : prediction.DnfPrediction.DnfPickedDrivers!.Intersect(race.Result.DnfDrivers).Count()
                                                * F1PredictionsPointsHelper.DnfPredictionPoints,
                                      SafetyCarsPoints = prediction.SafetyCarsPrediction == ToSafetyCarsEnum(race.Result.SafetyCars)
                                          ? F1PredictionsPointsHelper.IncidentsPredictionPoints
                                          : 0,
                                      TeamMatesPoints = prediction.TeamsPickedDrivers.Intersect(teamMatesWinners).Count(),
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
                x.TotalPoints = x.TenthPlacePoints + x.DnfsPoints + x.SafetyCarsPoints + x.FirstPlaceLeadPoints + x.TeamMatesPoints;
                if (race.IsSprint)
                {
                    x.TotalPoints = F1PredictionsPointsHelper.CalculateSprintPoints(x.TotalPoints);
                }

                return x;
            }
        ).ToArray();

        return results;
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

    private static string SelectHighestTeamMate(Dictionary<string, int> positions, string firstDriver, string secondDriver)
    {
        return positions.GetValueOrDefault(firstDriver, 999) < positions.GetValueOrDefault(secondDriver, 999)
            ? firstDriver
            : secondDriver;
    }
}