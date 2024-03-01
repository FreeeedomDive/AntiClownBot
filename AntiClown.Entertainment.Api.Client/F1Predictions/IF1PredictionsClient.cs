using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public interface IF1PredictionsClient
{
    Task<F1RaceDto> ReadAsync(Guid raceId);
    Task<Guid> StartNewRaceAsync(string name);
    Task ClosePredictionsAsync(Guid raceId);
    Task<Dictionary<Guid, F1PredictionUserResultDto?[]>> ReadStandingsAsync(int? season = null);
}