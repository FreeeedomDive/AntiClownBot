using AntiClown.Entertainment.Api.Client;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.EventsDaemon.Workers.F1Predictions;

public class F1PredictionStartWorker : ArbitraryIntervalPeriodicJobWorker
{
    public F1PredictionStartWorker(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IF1RacesProvider f1RacesProvider,
        ILoggerClient logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.f1RacesProvider = f1RacesProvider;
    }

    protected override Task<int?> TryGetMillisecondsBeforeNextIterationAsync()
    {
        var now = DateTime.UtcNow;
        var nextRace = f1RacesProvider
                       .GetRaces()
                       .OrderBy(x => x.PredictionsStartTime)
                       .FirstOrDefault(x => x.PredictionsStartTime > now);
        return nextRace is null
            ? Task.FromResult<int?>(null)
            : Task.FromResult((int?)(nextRace.PredictionsStartTime - now).TotalMilliseconds);
    }

    protected override async Task ExecuteIterationAsync()
    {
        var now = DateTime.UtcNow;
        var currentRace = f1RacesProvider
                       .GetRaces()
                       .OrderBy(x => (x.PredictionsStartTime - now).TotalMilliseconds)
                       .First();
        await antiClownEntertainmentApiClient.F1Predictions.StartNewRaceAsync(currentRace.Name);
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IF1RacesProvider f1RacesProvider;
}