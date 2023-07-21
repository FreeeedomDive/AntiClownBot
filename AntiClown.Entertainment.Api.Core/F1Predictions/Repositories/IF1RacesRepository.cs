using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

public interface IF1RacesRepository
{
    Task<F1Race> ReadAsync(Guid id);
    Task CreateAsync(F1Race race);
    Task<F1Race[]> ReadAllAsync();
    Task UpdateAsync(F1Race race);
}