/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.F1ChampionshipPredictions;

public interface IF1ChampionshipPredictionsClient
{
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipPredictionDto> ReadAsync(System.Guid userId, int season);
    Task CreateOrUpdateAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipPredictionDto dto);
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipResultsDto> ReadResultsAsync(int season);
    Task WriteResultsAsync(int season, AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipResultsDto dto);
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipUserPointsDto[]> BuildPointsAsync(int season);
}
