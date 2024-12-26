using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

public interface IF1PredictionTeamsRepository
{
    Task<F1Team[]> ReadAllAsync();
    Task CreateOrUpdateAsync(F1Team team);
}