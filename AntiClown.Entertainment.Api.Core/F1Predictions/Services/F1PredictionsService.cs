using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public class F1PredictionsService : IF1PredictionsService
{
    public F1PredictionsService(
        IF1RacesRepository f1RacesRepository,
        IF1PredictionResultsRepository f1PredictionResultsRepository
    )
    {
        this.f1RacesRepository = f1RacesRepository;
        this.f1PredictionResultsRepository = f1PredictionResultsRepository;
    }

    public async Task<F1Race> ReadAsync(Guid raceId)
    {
        return await f1RacesRepository.ReadAsync(raceId);
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
                FirstDnf = null,
            },
            Predictions = new List<F1Prediction>(),
        };
        await f1RacesRepository.CreateAsync(newRace);

        return raceId;
    }

    public async Task AddPredictionAsync(Guid raceId, Guid userId, F1Driver tenthPlaceDriver, F1Driver firstDnfDriver)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        if (!race.IsOpened)
        {
            throw new PredictionsAlreadyClosedException(raceId);
        }

        var userPrediction = race.Predictions.FirstOrDefault(x => x.UserId == userId);
        if (userPrediction is null)
        {
            race.Predictions.Add(
                new F1Prediction
                {
                    RaceId = raceId,
                    UserId = userId,
                    TenthPlacePickedDriver = tenthPlaceDriver,
                    FirstDnfPickedDriver = firstDnfDriver,
                }
            );
        }
        else
        {
            userPrediction.TenthPlacePickedDriver = tenthPlaceDriver;
            userPrediction.FirstDnfPickedDriver = firstDnfDriver;
        }

        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task ClosePredictionsAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.IsOpened = false;
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task AddFirstDnfResultAsync(Guid raceId, F1Driver firstDnfDriver)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.Result.FirstDnf = firstDnfDriver;
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task AddClassificationsResultAsync(Guid raceId, F1Driver[] f1Drivers)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        race.Result.Classification = f1Drivers;
        await f1RacesRepository.UpdateAsync(race);
    }

    public async Task<F1PredictionResult[]> FinishRaceAsync(Guid raceId)
    {
        var race = await f1RacesRepository.ReadAsync(raceId);
        var position = 1;
        var driverToPosition = race.Result.Classification.ToDictionary(
            x => x, _ =>
            {
                var pos = position++;
                return pos;
            }
        );
        var participantsResults = race.Predictions.Select(
            x => new F1PredictionResult
            {
                RaceId = raceId,
                UserId = x.UserId,
                FirstDnfPoints = x.FirstDnfPickedDriver == race.Result.FirstDnf ? F1PredictionsPointsHelper.PointsForCorrectFirstDnfPrediction : 0,
                TenthPlacePoints = F1PredictionsPointsHelper.PointsDistribution.TryGetValue(
                    driverToPosition.TryGetValue(x.TenthPlacePickedDriver, out var driverPosition) ? driverPosition : 0,
                    out var points
                )
                    ? points
                    : 0,
            }
        ).ToArray();
        await f1PredictionResultsRepository.CreateAsync(participantsResults);

        race.IsOpened = false;
        race.IsActive = false;
        await f1RacesRepository.UpdateAsync(race);
        
        return participantsResults;
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
    private readonly IF1RacesRepository f1RacesRepository;
}