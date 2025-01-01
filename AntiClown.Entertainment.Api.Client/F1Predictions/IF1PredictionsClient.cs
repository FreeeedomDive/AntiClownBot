/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public interface IF1PredictionsClient
{
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto[]> FindAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceFilterDto filter);
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1RaceDto> ReadAsync(System.Guid raceId);
    Task<System.Guid> StartNewRaceAsync(string name, bool isSprint);
    Task AddPredictionAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionDto prediction);
    Task ClosePredictionsAsync(System.Guid raceId);
    Task AddResultAsync(System.Guid raceId, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionRaceResultDto raceResult);
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> FinishRaceAsync(System.Guid raceId);
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto[]> ReadResultsAsync(System.Guid raceId);
    Task<Dictionary<System.Guid, AntiClown.Entertainment.Api.Dto.F1Predictions.F1PredictionUserResultDto?[]>> ReadStandingsAsync(int? season = null);
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.F1TeamDto[]> ReadTeamsAsync();
    Task CreateOrUpdateTeamAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.F1TeamDto dto);
}
