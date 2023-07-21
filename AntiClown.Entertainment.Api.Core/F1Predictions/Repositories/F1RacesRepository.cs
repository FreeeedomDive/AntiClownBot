using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

public class F1RacesRepository : IF1RacesRepository
{
    public F1RacesRepository(IVersionedSqlRepository<F1RaceStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<F1Race> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return ToModel(result);
    }

    public async Task CreateAsync(F1Race race)
    {
        var storageElement = ToStorageElement(race);
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task<F1Race[]> ReadAllAsync()
    {
        var result = await sqlRepository.ReadAllAsync();
        return result.Select(ToModel).ToArray();
    }

    public async Task UpdateAsync(F1Race race)
    {
        var storageElement = ToStorageElement(race);
        await sqlRepository.ConcurrentUpdateAsync(
            race.Id, x =>
            {
                x.IsActive = storageElement.IsActive;
                x.IsOpened = storageElement.IsOpened;
                x.SerializedPredictions = storageElement.SerializedPredictions;
                x.SerializedResults = storageElement.SerializedResults;
            }
        );
    }

    private static F1RaceStorageElement ToStorageElement(F1Race race)
    {
        return new F1RaceStorageElement
        {
            Id = race.Id,
            IsActive = race.IsActive,
            IsOpened = race.IsOpened,
            SerializedPredictions = JsonConvert.SerializeObject(race.Predictions),
            SerializedResults = JsonConvert.SerializeObject(race.Result),
        };
    }

    private static F1Race ToModel(F1RaceStorageElement storageElement)
    {
        return new F1Race
        {
            Id = storageElement.Id,
            IsActive = storageElement.IsActive,
            IsOpened = storageElement.IsOpened,
            Predictions = JsonConvert.DeserializeObject<List<F1Prediction>>(storageElement.SerializedPredictions)!,
            Result = JsonConvert.DeserializeObject<F1PredictionRaceResult>(storageElement.SerializedResults)!,
        };
    }

    private readonly IVersionedSqlRepository<F1RaceStorageElement> sqlRepository;
}