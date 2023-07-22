using AntiClown.Entertainment.Api.Dto.F1Predictions;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public interface IF1PredictionsClient
{
    Task<F1RaceDto> ReadAsync(Guid raceId);
    Task<Guid> StartNewRaceAsync(string name);
    Task AddPredictionAsync(Guid raceId, Guid userId, F1DriverDto tenthPlaceDriver, F1DriverDto firstDnfDriver);
    Task ClosePredictionsAsync(Guid raceId);
    Task AddFirstDnfResultAsync(Guid raceId, F1DriverDto firstDnfDriver);
    Task AddClassificationsResultAsync(Guid raceId, F1DriverDto[] f1Drivers);
    Task<F1PredictionResultDto[]> FinishRaceAsync(Guid raceId);
    Task<Dictionary<Guid, F1PredictionResultDto[]>> ReadStandingsAsync();
}