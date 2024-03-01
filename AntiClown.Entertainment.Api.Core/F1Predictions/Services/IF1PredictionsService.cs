using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public interface IF1PredictionsService
{
    Task<F1Race> ReadAsync(Guid raceId);
    Task<F1Race[]> ReadActiveAsync();
    Task<Guid> StartNewRaceAsync(string name);
    Task AddPredictionAsync(Guid raceId, Guid userId, F1Prediction prediction);
    Task ClosePredictionsAsync(Guid raceId);
    Task AddFirstDnfResultAsync(Guid raceId, F1Driver firstDnfDriver);
    Task AddClassificationsResultAsync(Guid raceId, F1Driver[] f1Drivers);
    Task AddRaceResultAsync(Guid raceId, F1PredictionRaceResult raceResult);
    Task<F1PredictionResult[]> FinishRaceAsync(Guid raceId);
    Task<Dictionary<Guid, F1PredictionResult?[]>> ReadStandingsAsync(int? season = null);
}