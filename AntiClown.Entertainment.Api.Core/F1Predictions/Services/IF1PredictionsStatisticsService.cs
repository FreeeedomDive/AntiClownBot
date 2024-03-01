using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public interface IF1PredictionsStatisticsService
{
    Task<MostPickedDriversStats> GetMostPickedDriversAsync();
    Task<MostPickedDriversStats> GetMostPickedDriversAsync(Guid userId);
    Task<MostProfitableDriversStats> GetMostProfitableDriversAsync();
    Task<UserPointsStats> GetUserPointsStatsAsync(Guid userId);
}