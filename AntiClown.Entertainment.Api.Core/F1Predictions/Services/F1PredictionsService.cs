using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public class F1PredictionsService : IF1PredictionsService
{
    public F1PredictionsService(
        IF1RacesRepository f1RacesRepository,
        IF1PredictionResultsRepository f1PredictionResultsRepository,
        IF1PredictionsMessageProducer f1PredictionsMessageProducer
    )
    {
        this.f1RacesRepository = f1RacesRepository;
        this.f1PredictionResultsRepository = f1PredictionResultsRepository;
        this.f1PredictionsMessageProducer = f1PredictionsMessageProducer;
    }

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

    public async Task<Guid> StartNewRaceAsync(string name)
    {
        var raceId = Guid.NewGuid();
        var newRace = new F1Race
        {
            Id = raceId,
            Season = DateTime.UtcNow.Year,
            Name = name,
            IsActive = true,
            IsOpened = true,
            Result = new F1PredictionRaceResult
            {
                RaceId = raceId,
                Classification = Array.Empty<F1Driver>(),
                DnfDrivers = Array.Empty<F1Driver>(),
                SafetyCars = 0,
                FirstPlaceLead = 0,
            },
            Predictions = new List<F1Prediction>(),
        };
        await f1RacesRepository.CreateAsync(newRace);

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

    public async Task AddClassificationsResultAsync(Guid raceId, F1Driver[] f1Drivers)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.Result.Classification = f1Drivers;
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task AddDnfDriverAsync(Guid raceId, F1Driver dnfDriver)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        var dnfs = race.Result.DnfDrivers.ToList();
        dnfs.Add(dnfDriver);
        race.Result.DnfDrivers = dnfs.ToArray();
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task AddSafetyCarAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.Result.SafetyCars++;
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task AddFirstPlaceLeadAsync(Guid raceId, decimal firstPlaceLead)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.Result.FirstPlaceLead = firstPlaceLead;
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task<F1PredictionResult[]> FinishRaceAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);

        var results = F1PredictionsResultBuilder.Build(race);

        await f1PredictionResultsRepository.CreateAsync(results);

        race.IsOpened = false;
        race.IsActive = false;
        await f1RacesRepository.UpdateAsync(race);
        await f1PredictionsMessageProducer.ProduceRaceFinishedAsync(raceId);

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
                Season = season ?? DateTime.UtcNow.Year,
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

    private readonly IF1PredictionResultsRepository f1PredictionResultsRepository;
    private readonly IF1PredictionsMessageProducer f1PredictionsMessageProducer;
    private readonly IF1RacesRepository f1RacesRepository;
}