using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.ChampionshipPredictions;

public interface IF1ChampionshipPredictionsRepository
{
    Task<F1ChampionshipPrediction> ReadAsync(Guid userId, int season);
    Task<F1ChampionshipPrediction[]> ReadAllAsync(int season);
    Task CreateOrUpdateAsync(F1ChampionshipPrediction prediction);
    Task<F1ChampionshipResults> ReadResultsAsync(int season);
    Task WriteResultsAsync(int season, F1ChampionshipResults results);
}
