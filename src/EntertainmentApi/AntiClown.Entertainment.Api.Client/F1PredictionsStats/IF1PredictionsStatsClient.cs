/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.F1PredictionsStats;

public interface IF1PredictionsStatsClient
{
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics.F1SeasonStatsDto> GetSeasonStatsAsync(int season);
}
