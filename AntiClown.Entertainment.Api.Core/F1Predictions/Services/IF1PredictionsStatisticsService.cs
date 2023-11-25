using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services;

public interface IF1PredictionsStatisticsService
{
    Task<MostPickedDriversByUsersStats> GetMostPickedDriversByUsersAsync();
    Task<MostProfitableDriversStats> GetMostProfitableDriversAsync();
}