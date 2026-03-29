using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Statistics;

public interface IF1PredictionsStatisticsService
{
    Task<F1SeasonStats> GetSeasonStatsAsync(int season);
}
