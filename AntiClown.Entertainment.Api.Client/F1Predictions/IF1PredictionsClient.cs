using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public interface IF1PredictionsClient
{
    Task<F1RaceDto[]> FindAsync(F1RaceFilterDto filter);
    Task<F1RaceDto> ReadAsync(Guid raceId);
    Task<Guid> StartNewRaceAsync(string name);
    Task ClosePredictionsAsync(Guid raceId);
    Task AddPredictionAsync(Guid raceId, F1PredictionDto f1Prediction);
    Task AddResultAsync(Guid raceId, F1PredictionRaceResultDto result);
    Task<F1PredictionUserResultDto[]> FinishAsync(Guid raceId);
    Task<F1PredictionUserResultDto[]> ReadResultsAsync(Guid raceId);
    Task<Dictionary<Guid, F1PredictionUserResultDto?[]>> ReadStandingsAsync(int? season = null);
}