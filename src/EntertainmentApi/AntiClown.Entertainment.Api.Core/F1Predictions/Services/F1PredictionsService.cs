using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;
using AntiClown.Entertainment.Api.Core.F1Predictions.Options;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Teams;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;
using AntiClown.Tools.Utility.Extensions;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public class F1PredictionsService(
    IF1RacesRepository f1RacesRepository,
    IF1PredictionResultsRepository f1PredictionResultsRepository,
    IF1PredictionsMessageProducer f1PredictionsMessageProducer,
    IF1PredictionTeamsRepository f1PredictionTeamsRepository,
    IF1PredictionsResultBuilder f1PredictionsResultBuilder,
    IF1ChampionshipPredictionsService championshipPredictionsService,
    IJolpicaClient jolpicaClient,
    IScheduler scheduler,
    IOptions<F1PredictionsOptions> options,
    ILogger<F1PredictionsService> logger,
    TimeProvider timeProvider
) : IF1PredictionsService
{
    public async Task<F1Race> ReadAsync(Guid raceId)
    {
        return await f1RacesRepository.ReadAsync(raceId);
    }

    public async Task<F1Race[]> ReadActiveAsync()
    {
        return await f1RacesRepository.FindAsync(
            new F1RaceFilter
            {
                IsActive = true,
            }
        );
    }

    public async Task<F1Race[]> FindAsync(F1RaceFilter filter)
    {
        return await f1RacesRepository.FindAsync(filter);
    }

    public async Task<Guid> StartNewRaceAsync(string name, bool isSprint)
    {
        var raceId = Guid.NewGuid();
        var positionPredictionDriver = (await GetActiveTeamsAsync())
                                       .SelectMany(x => new[] { x.FirstDriver, x.SecondDriver })
                                       .SelectRandomItem();
        var newRace = new F1Race
        {
            Id = raceId,
            Season = timeProvider.GetUtcNow().Year,
            Name = name,
            IsSprint = isSprint,
            IsActive = true,
            IsOpened = true,
            Conditions = new PredictionConditions
            {
                PositionPredictionDriver = positionPredictionDriver,
            },
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = [],
                DnfDrivers = [],
                SafetyCars = 0,
                FirstPlaceLead = 0,
            },
            Predictions = [],
        };
        await f1RacesRepository.CreateAsync(newRace);
        await f1PredictionsMessageProducer.ProducePredictionStartedAsync(raceId);

        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => PollQualifyingGridAsync(raceId),
                TimeSpan.Zero
            )
        );

        return raceId;
    }

    public async Task AddPredictionAsync(Guid raceId, Guid userId, F1Prediction prediction)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        if (!race.IsOpened)
        {
            throw new PredictionsAlreadyClosedException(raceId);
        }

        prediction.RaceId = raceId;
        var deletedCount = race.Predictions.RemoveAll(x => x.UserId == userId);
        race.Predictions.Add(prediction);
        await f1RacesRepository.UpdateAsync(race);
        await f1PredictionsMessageProducer.ProducePredictionUpdatedAsync(userId, raceId, deletedCount == 0);
    }

    public async Task ClosePredictionsAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.IsOpened = false;
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task AddRaceResultAsync(Guid raceId, F1PredictionRaceResult raceResult)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.Result = raceResult;
        await f1RacesRepository.UpdateAsync(race);
        await f1PredictionsMessageProducer.ProduceRaceResultUpdatedAsync(raceId);
    }

    public async Task<F1PredictionResult[]> FinishRaceAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);

        var results = f1PredictionsResultBuilder.Build(race);

        await f1PredictionResultsRepository.CreateOrUpdateAsync(results);

        race.IsOpened = false;
        race.IsActive = false;
        await f1RacesRepository.UpdateAsync(race);
        await f1PredictionsMessageProducer.ProduceRaceFinishedAsync(raceId);

        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => PollChampionshipResultsAsync(raceId),
                options.Value.ChampionshipPollingInterval
            )
        );

        return results;
    }

    public async Task<F1PredictionResult[]> ReadRaceResultsAsync(Guid raceId)
    {
        return await f1PredictionResultsRepository.FindAsync(
            new F1PredictionResultsFilter
            {
                RaceId = raceId,
            }
        );
    }

    public async Task<Dictionary<Guid, F1PredictionResult?[]>> ReadStandingsAsync(int? season = null)
    {
        var finishedRacesOfThisSeason = await f1RacesRepository.FindAsync(
            new F1RaceFilter
            {
                Season = season ?? timeProvider.GetUtcNow().Year,
                IsActive = false,
            }
        );
        var totalRaces = finishedRacesOfThisSeason.Length;
        var result = new Dictionary<Guid, F1PredictionResult?[]>();
        for (var currentRaceIndex = 0; currentRaceIndex < totalRaces; currentRaceIndex++)
        {
            var currentRace = finishedRacesOfThisSeason[currentRaceIndex];
            var currentRacePredictionsResults = await f1PredictionResultsRepository.FindAsync(
                new F1PredictionResultsFilter
                {
                    RaceId = currentRace.Id,
                }
            );
            foreach (var prediction in currentRace.Predictions)
            {
                if (!result.TryGetValue(prediction.UserId, out _))
                {
                    result.Add(prediction.UserId, new F1PredictionResult?[totalRaces]);
                }

                result[prediction.UserId][currentRaceIndex] = currentRacePredictionsResults.First(x => x.UserId == prediction.UserId);
            }
        }

        return result;
    }

    public async Task<F1Team[]> GetActiveTeamsAsync()
    {
        return await f1PredictionTeamsRepository.ReadAllAsync();
    }

    public async Task CreateOrUpdateTeamAsync(F1Team team)
    {
        await f1PredictionTeamsRepository.CreateOrUpdateAsync(team);
    }

    public async Task SaveQualifyingGridAsync(Guid raceId, string[] grid)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.QualifyingGrid = grid;
        await f1RacesRepository.UpdateAsync(race);
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task PollQualifyingGridAsync(Guid raceId)
    {
        var found = await TryLoadQualifyingGridAsync(raceId);
        if (!found)
        {
            scheduler.Schedule(
                () => BackgroundJob.Schedule(
                    () => PollQualifyingGridAsync(raceId),
                    options.Value.QualifyingGridPollingInterval
                )
            );
        }
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task PollChampionshipResultsAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);

        // Sprint races don't affect driver championship standings round count
        if (race.IsSprint)
        {
            logger.LogInformation("Race {RaceId} is a sprint, skipping championship standings poll", raceId);
            return;
        }

        var finishedRaces = await f1RacesRepository.FindAsync(new F1RaceFilter { Season = race.Season, IsActive = false });
        var expectedRound = finishedRaces.Count(x => !x.IsSprint);

        var result = await jolpicaClient.GetDriverStandingsAsync(race.Season);

        if (result is null || result.Value.Round < expectedRound)
        {
            logger.LogInformation(
                "Championship standings for season {Season} not yet updated (got round {GotRound}, expected {ExpectedRound}), rescheduling",
                race.Season, result?.Round, expectedRound
            );
            scheduler.Schedule(
                () => BackgroundJob.Schedule(
                    () => PollChampionshipResultsAsync(raceId),
                    options.Value.ChampionshipPollingInterval
                )
            );
            return;
        }

        var existing = await championshipPredictionsService.ReadResultsAsync(race.Season);
        existing.Standings = result.Value.Standings;
        await championshipPredictionsService.WriteResultsAsync(race.Season, existing);

        logger.LogInformation(
            "Championship standings for season {Season} updated after round {Round}",
            race.Season, result.Value.Round
        );
    }

    private async Task<bool> TryLoadQualifyingGridAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        var raceIndex = (await f1RacesRepository.FindAsync(new F1RaceFilter { Season = race.Season })).Count(x => !x.IsSprint);

        var qualifyingNames = await jolpicaClient.GetQualifyingDriverNamesAsync(race.Season, raceIndex);
        if (qualifyingNames is null || qualifyingNames.Length == 0)
        {
            return false;
        }

        var allDrivers = (await f1PredictionTeamsRepository.ReadAllAsync())
                         .SelectMany(t => new[] { t.FirstDriver, t.SecondDriver })
                         .ToArray();
        var qualifyingSet = qualifyingNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var backOfGridDrivers = allDrivers.Where(d => !qualifyingSet.Contains(d)).ToArray();

        race.QualifyingGrid = qualifyingNames.Concat(backOfGridDrivers).ToArray();
        await f1RacesRepository.UpdateAsync(race);
        return true;
    }
}