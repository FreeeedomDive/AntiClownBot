using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories;

public interface IF1PredictionResultsRepository
{
    Task CreateOrUpdateAsync(F1PredictionResult[] results);
    Task<F1PredictionResult[]> FindAsync(F1PredictionResultsFilter filter);
}