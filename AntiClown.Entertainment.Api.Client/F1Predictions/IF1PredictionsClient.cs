using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public interface IF1PredictionsClient
{
    Task<F1RaceDto> ReadAsync(Guid raceId);
    Task<Guid> StartNewRaceAsync(string name);
    Task ClosePredictionsAsync(Guid raceId);
    Task AddClassificationsResultAsync(Guid raceId, F1DriverDto[] f1Drivers);
    Task AddDnfDriverAsync(Guid raceId, F1DriverDto driver);
    Task AddSafetyCarAsync(Guid raceId);
    Task AddFirstPlaceLeadAsync(Guid raceId, decimal firstPlaceLead);
    Task<F1PredictionUserResultDto[]> FinishAsync(Guid raceId);
    Task<Dictionary<Guid, F1PredictionUserResultDto?[]>> ReadStandingsAsync(int? season = null);
}