using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
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

    protected override async Task<int?> TryGetMillisecondsBeforeNextIterationAsync()
    {
        var now = DateTime.UtcNow;
        var currentRacesNames = (await antiClownEntertainmentApiClient.F1Predictions.FindAsync(
            new F1RaceFilterDto
            {
                Season = now.Year,
                IsActive = true,
            }
        )).Select(x => x.Name).ToArray();
        var nextRace = f1RacesProvider
                       .GetRaces()
                       .OrderBy(x => x.PredictionsStartTime)
                       .FirstOrDefault(
                           x =>
                               x.PredictionsStartTime > now && !currentRacesNames.Contains(x.Name)
                       );
        return nextRace is null
            ? null
            : (int?)(nextRace.PredictionsStartTime - now).TotalMilliseconds;
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

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IF1RacesProvider f1RacesProvider;
}