using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public class F1PredictionsStatisticsService
{
    public F1PredictionsStatisticsService(
        IF1RacesRepository f1RacesRepository,
        IF1PredictionResultsRepository f1PredictionResultsRepository
    )
    {
        this.f1RacesRepository = f1RacesRepository;
        this.f1PredictionResultsRepository = f1PredictionResultsRepository;
    }

    public async Task<MostPickedDriversByUsersStats> GetMostPickedDriversByUsersAsync()
    {
        var finishedRaces = (await f1RacesRepository.ReadAllAsync()).Where(x => !x.IsActive).ToArray();
        var predictions = finishedRaces.SelectMany(x => x.Predictions).ToArray();
        var tenthPlacePredictions = CountAndOrderByScore(predictions, x => x.TenthPlacePickedDriver);
        var dnfPredictions = CountAndOrderByScore(predictions, x => x.FirstDnfPickedDriver);

        return new MostPickedDriversByUsersStats
        {
            TenthPlacePickedDrivers = tenthPlacePredictions,
            FirstDnfPickedDrivers = dnfPredictions,
        };
    }

    public async Task<MostProfitableDriversStats> GetMostProfitableDriversAsync()
    {
        var finishedRaces = (await f1RacesRepository.ReadAllAsync()).Where(x => !x.IsActive).ToArray();
        var raceResults = finishedRaces.Select(x => x.Result).ToArray();
        var tenthPlacePoints = Enum.GetValues<F1Driver>().ToDictionary(driver => driver, _ => 0);
        foreach (var raceResult in raceResults)
        {
            var position = 0;
            foreach (var driver in raceResult.Classification)
            {
                position++;
                tenthPlacePoints[driver] += F1PredictionsPointsHelper.PointsDistribution[position];
            }
        }

        var correctedTenthPlacePoints = tenthPlacePoints
                                        .Where(kv => kv.Value > 0)
                                        .Select(
                                            kv => new DriverStatistics
                                            {
                                                Driver = kv.Key,
                                                Score = kv.Value,
                                            }
                                        ).ToArray();
        var tenthPlaceCount = CountAndOrderByScore(raceResults, x => x.Classification[9]);
        var firstDnfCount = CountAndOrderByScore(raceResults, x => x.FirstDnf);

        return new MostProfitableDriversStats
        {
            TenthPlacePoints = correctedTenthPlacePoints,
            TenthPlaceCount = tenthPlaceCount,
            FirstDnfCount = firstDnfCount,
        };
    }

    private static DriverStatistics[] CountAndOrderByScore<T>(IEnumerable<T> predictions, Func<T, F1Driver?> selector)
    {
        return predictions
               .GroupBy(selector)
               .Where(x => x.Key is not null)
               .Select(
                   group => new DriverStatistics
                   {
                       Driver = group.Key!.Value,
                       Score = group.Count(),
                   }
               )
               .OrderByDescending(x => x.Score)
               .ToArray();
    }

    private readonly IF1PredictionResultsRepository f1PredictionResultsRepository;
    private readonly IF1RacesRepository f1RacesRepository;
}