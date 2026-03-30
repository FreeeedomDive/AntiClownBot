using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;
using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Statistics;

public class F1PredictionsStatisticsService(
    IF1RacesRepository f1RacesRepository,
    IF1PredictionResultsRepository f1PredictionResultsRepository
) : IF1PredictionsStatisticsService
{
    public async Task<F1SeasonStats> GetSeasonStatsAsync(int season)
    {
        var finishedRaces = await f1RacesRepository.FindAsync(new F1RaceFilter
        {
            Season = season,
            IsActive = false,
        });

        if (finishedRaces.Length == 0)
        {
            return EmptyStats();
        }

        var finishedRaceIds = finishedRaces.Select(x => x.Id).ToHashSet();
        var allPredictionResults = await f1PredictionResultsRepository.FindAsync(new F1PredictionResultsFilter());
        var seasonResults = allPredictionResults.Where(x => finishedRaceIds.Contains(x.RaceId)).ToArray();
        var resultLookup = seasonResults.ToDictionary(x => (x.RaceId, x.UserId));

        var allPredictions = finishedRaces.SelectMany(x => x.Predictions).ToArray();
        var raceResultById = finishedRaces.ToDictionary(x => x.Id, x => x.Result);

        var tenthPlacePointsRating = allPredictions
            .GroupBy(p => p.TenthPlacePickedDriver)
            .Select(g => new DriverStatistics
            {
                Driver = g.Key,
                Score = g.Sum(p => resultLookup.TryGetValue((p.RaceId, p.UserId), out var r) ? r.TenthPlacePoints : 0),
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(TopCount)
            .ToArray();

        var mostPickedForTenthPlace = allPredictions
            .GroupBy(p => p.TenthPlacePickedDriver)
            .Select(g => new DriverStatistics
            {
                Driver = g.Key,
                Score = g.Count(),
            })
            .OrderByDescending(x => x.Score)
            .Take(TopCount)
            .ToArray();

        var tenthPickedButDnfed = allPredictions
            .Where(p => raceResultById.TryGetValue(p.RaceId, out var result) && result.DnfDrivers.Contains(p.TenthPlacePickedDriver))
            .GroupBy(p => p.TenthPlacePickedDriver)
            .Select(g => new DriverStatistics
            {
                Driver = g.Key,
                Score = g.Count(),
            })
            .OrderByDescending(x => x.Score)
            .Take(TopCount)
            .ToArray();

        var driverOwnTenthPlacePoints = finishedRaces
            .SelectMany(r => r.Result.Classification.Select((driver, index) => new
            {
                Driver = driver,
                Points = F1PredictionsHelper.PointsByFinishPlaceDistribution.GetValueOrDefault(index + 1),
            }))
            .GroupBy(x => x.Driver)
            .Select(g => new DriverStatistics
            {
                Driver = g.Key,
                Score = g.Sum(x => x.Points),
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(TopCount)
            .ToArray();

        var tenthPlacePredictionEfficiency = allPredictions
            .GroupBy(p => p.TenthPlacePickedDriver)
            .Select(g => new
            {
                Driver = g.Key,
                TotalPoints = g.Sum(p => resultLookup.TryGetValue((p.RaceId, p.UserId), out var r) ? r.TenthPlacePoints : 0),
                Count = g.Count(),
            })
            .Where(x => x.Count > 0 && x.TotalPoints > 0)
            .Select(x => new DriverStatistics
            {
                Driver = x.Driver,
                Score = x.TotalPoints / x.Count,
            })
            .OrderByDescending(x => x.Score)
            .Take(TopCount)
            .ToArray();

        var mostDnfDrivers = finishedRaces
            .SelectMany(r => r.Result.DnfDrivers)
            .GroupBy(d => d)
            .Select(g => new DriverStatistics
            {
                Driver = g.Key,
                Score = g.Count(),
            })
            .OrderByDescending(x => x.Score)
            .Take(TopCount)
            .ToArray();

        var mostPickedForDnf = allPredictions
            .Where(p => p.DnfPrediction is { NoDnfPredicted: false, DnfPickedDrivers: { Length: > 0 } })
            .SelectMany(p => p.DnfPrediction.DnfPickedDrivers!)
            .GroupBy(d => d)
            .Select(g => new DriverStatistics
            {
                Driver = g.Key,
                Score = g.Count(),
            })
            .OrderByDescending(x => x.Score)
            .Take(TopCount)
            .ToArray();

        var closestLeadGapPredictions = finishedRaces
            .Where(r => r.Predictions.Count > 0)
            .SelectMany(r => r.Predictions.Select(p => new LeadGapPredictionStats
            {
                UserId = p.UserId,
                RaceName = r.Name,
                PredictedGap = p.FirstPlaceLeadPrediction,
                ActualGap = r.Result.FirstPlaceLead,
                Difference = Math.Abs(r.Result.FirstPlaceLead - p.FirstPlaceLeadPrediction),
            }))
            .OrderBy(x => x.Difference)
            .Take(TopCount)
            .ToArray();

        var safetyCarPickCounts = allPredictions
            .GroupBy(p => p.SafetyCarsPrediction)
            .Select(g => new DriverStatistics
            {
                Driver = SafetyCarVariantName(g.Key),
                Score = g.Count(),
            })
            .OrderByDescending(x => x.Score)
            .ToArray();

        var safetyCarActualCounts = finishedRaces
            .GroupBy(r => F1PredictionsResultBuilder.ToSafetyCarsEnum(r.Result.SafetyCars))
            .Select(g => new DriverStatistics
            {
                Driver = SafetyCarVariantName(g.Key),
                Score = g.Count(),
            })
            .OrderByDescending(x => x.Score)
            .ToArray();

        var safetyCarCorrectGuesses = allPredictions
            .Where(p => raceResultById.TryGetValue(p.RaceId, out var result) &&
                        p.SafetyCarsPrediction == F1PredictionsResultBuilder.ToSafetyCarsEnum(result.SafetyCars))
            .GroupBy(p => p.SafetyCarsPrediction)
            .Select(g => new DriverStatistics
            {
                Driver = SafetyCarVariantName(g.Key),
                Score = g.Count(),
            })
            .OrderByDescending(x => x.Score)
            .ToArray();

        return new F1SeasonStats
        {
            TenthPlacePointsRating = tenthPlacePointsRating,
            MostPickedForTenthPlace = mostPickedForTenthPlace,
            TenthPickedButDnfed = tenthPickedButDnfed,
            DriverOwnTenthPlacePoints = driverOwnTenthPlacePoints,
            TenthPlacePredictionEfficiency = tenthPlacePredictionEfficiency,
            MostDnfDrivers = mostDnfDrivers,
            MostPickedForDnf = mostPickedForDnf,
            ClosestLeadGapPredictions = closestLeadGapPredictions,
            SafetyCarPickCounts = safetyCarPickCounts,
            SafetyCarActualCounts = safetyCarActualCounts,
            SafetyCarCorrectGuesses = safetyCarCorrectGuesses,
        };
    }

    private static string SafetyCarVariantName(SafetyCarsCount variant) => variant switch
    {
        SafetyCarsCount.Zero => "Нет",
        SafetyCarsCount.One => "1",
        SafetyCarsCount.Two => "2",
        SafetyCarsCount.ThreePlus => "3+",
        _ => variant.ToString(),
    };

    private static F1SeasonStats EmptyStats() => new()
    {
        TenthPlacePointsRating = [],
        MostPickedForTenthPlace = [],
        TenthPickedButDnfed = [],
        DriverOwnTenthPlacePoints = [],
        TenthPlacePredictionEfficiency = [],
        MostDnfDrivers = [],
        MostPickedForDnf = [],
        ClosestLeadGapPredictions = [],
        SafetyCarPickCounts = [],
        SafetyCarActualCounts = [],
        SafetyCarCorrectGuesses = [],
    };

    private const int TopCount = 5;
}
