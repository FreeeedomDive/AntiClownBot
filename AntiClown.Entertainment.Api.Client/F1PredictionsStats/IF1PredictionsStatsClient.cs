/* Generated file */
namespace AntiClown.Entertainment.Api.Client.F1PredictionsStats;

public interface IF1PredictionsStatsClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync();
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostPickedDriversStatsDto> GetMostPickedDriversAsync(System.Guid userId);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.MostProfitableDriversStatsDto> GetMostProfitableDriversAsync();
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.UserPointsStatsDto> GetUserPointsStatsAsync(System.Guid userId);
}
