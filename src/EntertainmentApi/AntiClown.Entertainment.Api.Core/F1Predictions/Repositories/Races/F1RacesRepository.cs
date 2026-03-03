using AntiClown.Core.Serializers;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;

public class F1RacesRepository(
    IVersionedSqlRepository<F1RaceStorageElement> sqlRepository,
    IJsonSerializer jsonSerializer
) : IF1RacesRepository
{
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

    public async Task<F1Race[]> FindAsync(F1RaceFilter filter)
    {
        var queryable = await sqlRepository.BuildCustomQueryAsync();
        var result = await queryable
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
                x.Name = storageElement.Name;
                x.IsSprint = storageElement.IsSprint;
                x.IsActive = storageElement.IsActive;
                x.IsOpened = storageElement.IsOpened;
                x.SerializedConditions = storageElement.SerializedConditions;
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
            IsSprint = race.IsSprint,
            SerializedConditions = race.Conditions is null ? null : jsonSerializer.Serialize(race.Conditions),
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
            IsSprint = storageElement.IsSprint,
            Conditions = string.IsNullOrEmpty(storageElement.SerializedConditions)
                ? null
                : jsonSerializer.Deserialize<PredictionConditions>(storageElement.SerializedConditions),
            Predictions = jsonSerializer.Deserialize<List<F1Prediction>>(storageElement.SerializedPredictions),
            Result = jsonSerializer.Deserialize<F1PredictionRaceResult>(storageElement.SerializedResults),
        };
    }
}