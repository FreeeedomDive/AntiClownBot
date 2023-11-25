using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;

namespace AntiClown.Entertainment.Api.Client.F1Predictions;

public interface IF1PredictionsStatsClient
{
    Task<MostPickedDriversByUsersStatsDto> GetMostPickedDriversByUsersAsync();
    Task<MostProfitableDriversStatsDto> GetMostProfitableDriversAsync();
}