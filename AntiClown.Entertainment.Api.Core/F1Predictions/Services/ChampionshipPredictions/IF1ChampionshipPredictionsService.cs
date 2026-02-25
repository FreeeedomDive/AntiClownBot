using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;

public interface IF1ChampionshipPredictionsService
{
    Task<F1ChampionshipPrediction> ReadAsync(Guid userId, int season);
    Task CreateOrUpdateAsync(F1ChampionshipPrediction prediction);
    Task<F1ChampionshipResults> ReadResultsAsync(int season);
    Task WriteResultsAsync(int season, F1ChampionshipResults results);
}
