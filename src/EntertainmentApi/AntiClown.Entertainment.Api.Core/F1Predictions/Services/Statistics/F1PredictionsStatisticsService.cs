using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Statistics;

public class F1PredictionsStatisticsService : IF1PredictionsStatisticsService
{
    public F1PredictionsStatisticsService(
        IF1RacesRepository f1RacesRepository,
        IF1PredictionResultsRepository f1PredictionResultsRepository
    )
    {
        this.f1RacesRepository = f1RacesRepository;
        this.f1PredictionResultsRepository = f1PredictionResultsRepository;
    }

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

        return new F1SeasonStats
        {
            TenthPlacePointsRating = tenthPlacePointsRating,
            MostPickedForTenthPlace = mostPickedForTenthPlace,
            TenthPickedButDnfed = tenthPickedButDnfed,
            MostDnfDrivers = mostDnfDrivers,
            MostPickedForDnf = mostPickedForDnf,
            ClosestLeadGapPredictions = closestLeadGapPredictions,
        };
    }

    private static F1SeasonStats EmptyStats() => new()
    {
        TenthPlacePointsRating = [],
        MostPickedForTenthPlace = [],
        TenthPickedButDnfed = [],
        MostDnfDrivers = [],
        MostPickedForDnf = [],
        ClosestLeadGapPredictions = [],
    };

    private const int TopCount = 5;

    private readonly IF1RacesRepository f1RacesRepository;
    private readonly IF1PredictionResultsRepository f1PredictionResultsRepository;
}
