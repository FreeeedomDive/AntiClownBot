using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Results;

public interface IF1PredictionsResultBuilder
{
    Task<F1PredictionResult[]> Build(F1Race race);
}