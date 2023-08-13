using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

public interface IF1PredictionResultsRepository
{
    Task CreateAsync(F1PredictionResult[] results);
    Task<F1PredictionResult[]> FindAsync(F1PredictionResultsFilter filter);
}