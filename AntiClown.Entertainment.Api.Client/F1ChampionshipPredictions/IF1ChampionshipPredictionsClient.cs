/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.F1ChampionshipPredictions;

public interface IF1ChampionshipPredictionsClient
{
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipPredictionDto> ReadAsync(System.Guid userId, int season);
    Task CreateOrUpdateAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions.F1ChampionshipPredictionDto dto);
}
