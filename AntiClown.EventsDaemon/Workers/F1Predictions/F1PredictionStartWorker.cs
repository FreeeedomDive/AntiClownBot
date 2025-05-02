using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.EventsDaemon.Workers.F1Predictions;

public class F1PredictionStartWorker(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IF1RacesProvider f1RacesProvider,
    ILogger<F1PredictionStartWorker> logger
)
    : ArbitraryIntervalPeriodicJobWorker(logger)
{
    protected override async Task<TimeSpan?> TryGetMillisecondsBeforeNextIterationAsync()
    {
        var now = DateTime.UtcNow;
        var nextRace = f1RacesProvider
                       .GetRaces()
                       .OrderBy(x => x.PredictionsStartTime)
                       .FirstOrDefault(x => x.PredictionsStartTime > now);
        return nextRace is null
            ? null
            : nextRace.PredictionsStartTime - now;
    }

    protected override async Task ExecuteIterationAsync()
    {
        var now = DateTime.UtcNow;
        var currentRace = f1RacesProvider
                          .GetRaces()
                          .OrderBy(x => Math.Abs((x.PredictionsStartTime - now).TotalMilliseconds))
                          .First();
        await antiClownEntertainmentApiClient.F1Predictions.StartNewRaceAsync(currentRace.Name, currentRace.IsSprint);
    }
}