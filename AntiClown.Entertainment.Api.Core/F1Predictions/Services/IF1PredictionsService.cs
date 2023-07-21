using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public interface IF1PredictionsService
{
    Task<F1Race> ReadAsync(Guid raceId);
    Task<Guid> StartNewRaceAsync(string name);
    Task AddPredictionAsync(Guid raceId, Guid userId, F1Driver tenthPlaceDriver, F1Driver firstDnfDriver);
    Task ClosePredictionsAsync(Guid raceId);
    Task AddFirstDnfResultAsync(Guid raceId, F1Driver firstDnfDriver);
    Task AddClassificationsResultAsync(Guid raceId, F1Driver[] f1Drivers);
    Task<F1PredictionResult[]> FinishRaceAsync(Guid raceId);
}