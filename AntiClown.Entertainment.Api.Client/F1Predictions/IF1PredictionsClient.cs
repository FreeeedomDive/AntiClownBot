/* Generated file */
namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public interface IF1PredictionsClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto[]> FindAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceFilterDto filter);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto> ReadAsync(System.Guid raceId);
    System.Threading.Tasks.Task<System.Guid> StartNewRaceAsync(System.String name);
    System.Threading.Tasks.Task AddPredictionAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionDto prediction);
    System.Threading.Tasks.Task ClosePredictionsAsync(System.Guid raceId);
    System.Threading.Tasks.Task AddResultAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionRaceResultDto raceResult);
    System.Threading.Tasks.Task AddClassificationsResultAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1DriverDto[] f1Drivers);
    System.Threading.Tasks.Task AddDnfDriverAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1DriverDto dnfDriver);
    System.Threading.Tasks.Task AddSafetyCarAsync(System.Guid raceId);
    System.Threading.Tasks.Task AddFirstPlaceLeadAsync(System.Guid raceId, System.Decimal firstPlaceLead);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> FinishRaceAsync(System.Guid raceId);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> ReadResultsAsync(System.Guid raceId);
    System.Threading.Tasks.Task<Dictionary<System.Guid, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]>> ReadStandingsAsync(System.Int32? season = null);
}
