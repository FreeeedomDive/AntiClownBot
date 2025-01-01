using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public interface IF1PredictionsService
{
    Task<F1Race> ReadAsync(Guid raceId);
    Task<F1Race[]> ReadActiveAsync();
    Task<F1Race[]> FindAsync(F1RaceFilter filter);
    Task<Guid> StartNewRaceAsync(string name, bool isSprint);
    Task AddPredictionAsync(Guid raceId, Guid userId, F1Prediction prediction);
    Task ClosePredictionsAsync(Guid raceId);
    Task AddRaceResultAsync(Guid raceId, F1PredictionRaceResult raceResult);
    Task<F1PredictionResult[]> FinishRaceAsync(Guid raceId);
    Task<F1PredictionResult[]> ReadRaceResultsAsync(Guid raceId);
    Task<Dictionary<Guid, F1PredictionResult?[]>> ReadStandingsAsync(int? season = null);
    Task<F1Team[]> GetActiveTeamsAsync();
    Task CreateOrUpdateTeamAsync(F1Team team);
}