using AntiClown.Core.Serializers;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

public class F1RacesRepository : IF1RacesRepository
{
    public F1RacesRepository(
        IVersionedSqlRepository<F1RaceStorageElement> sqlRepository,
        IJsonSerializer jsonSerializer
    )
    {
        this.sqlRepository = sqlRepository;
        this.jsonSerializer = jsonSerializer;
    }

    public async Task<F1Race> ReadAsync(Guid id)
    {
        var result = await sqlRepository.ReadAsync(id);
        return ToModel(result);
    }

    public async Task CreateAsync(F1Race race)
    {
        var storageElement = ToStorageElement(race);
        storageElement.CreatedAt = DateTime.UtcNow;
        await sqlRepository.CreateAsync(storageElement);
    }

    public async Task<F1Race[]> ReadAllAsync()
    {
        var result = await sqlRepository
                           .BuildCustomQuery()
                           .OrderBy(x => x.CreatedAt)
                           .ToArrayAsync();
        return result.Select(ToModel).ToArray();
    }

    public async Task<F1Race[]> FindAsync(F1RaceFilter filter)
    {
        var result = await sqlRepository
                           .BuildCustomQuery()
                           .WhereIf(filter.Name is not null, x => x.Name == filter.Name)
                           .WhereIf(filter.Season is not null, x => x.Season == filter.Season!.Value)
                           .WhereIf(filter.IsActive is not null, x => x.IsActive == filter.IsActive!.Value)
                           .OrderBy(x => x.CreatedAt)
                           .ToArrayAsync();
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

    private F1RaceStorageElement ToStorageElement(F1Race race)
    {
        return new F1RaceStorageElement
        {
            Id = race.Id,
            Season = race.Season,
            Name = race.Name,
            IsActive = race.IsActive,
            IsOpened = race.IsOpened,
            SerializedPredictions = jsonSerializer.Serialize(race.Predictions),
            SerializedResults = jsonSerializer.Serialize(race.Result),
        };
    }

    private F1Race ToModel(F1RaceStorageElement storageElement)
    {
        return new F1Race
        {
            Id = storageElement.Id,
            Season = storageElement.Season,
            Name = storageElement.Name,
            IsActive = storageElement.IsActive,
            IsOpened = storageElement.IsOpened,
            Predictions = JsonConvert.DeserializeObject<List<F1Prediction>>(storageElement.SerializedPredictions)!,
            Result = JsonConvert.DeserializeObject<F1PredictionRaceResult>(storageElement.SerializedResults)!,
        };
    }

    private readonly IVersionedSqlRepository<F1RaceStorageElement> sqlRepository;
    private readonly IJsonSerializer jsonSerializer;
}