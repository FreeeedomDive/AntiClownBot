using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Races;

public interface IF1RacesRepository
{
    Task<F1Race> ReadAsync(Guid id);
    Task CreateAsync(F1Race race);
    Task<F1Race[]> FindAsync(F1RaceFilter filter);
    Task UpdateAsync(F1Race race);
}