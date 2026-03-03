/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.F1PredictionsStats;

public interface IF1PredictionsStatsClient
{
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync();
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync(System.Guid userId);
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostProfitableDriversStatsDto> GetMostProfitableDriversAsync();
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.UserPointsStatsDto> GetUserPointsStatsAsync(System.Guid userId);
}
