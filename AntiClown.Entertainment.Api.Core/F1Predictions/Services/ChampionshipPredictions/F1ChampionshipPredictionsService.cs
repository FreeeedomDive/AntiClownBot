using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.ChampionshipPredictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;

public class F1ChampionshipPredictionsService(IF1ChampionshipPredictionsRepository repository) : IF1ChampionshipPredictionsService
{
    public async Task<F1ChampionshipPrediction> ReadAsync(Guid userId, int season)
    {
        return await repository.ReadAsync(userId, season);
    }

    public async Task CreateOrUpdateAsync(F1ChampionshipPrediction prediction)
    {
        await repository.CreateOrUpdateAsync(prediction);
    }
}
