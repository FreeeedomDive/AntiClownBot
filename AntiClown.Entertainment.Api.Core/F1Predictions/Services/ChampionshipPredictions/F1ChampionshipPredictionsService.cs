using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;

public class F1ChampionshipPredictionsService(IF1ChampionshipPredictionsRepository repository) : IF1ChampionshipPredictionsService
{
    public async Task<F1ChampionshipPrediction> ReadAsync(Guid userId, int season)
    {
        return await repository.ReadAsync(userId, season);
    }

    public async Task CreateOrUpdateAsync(F1ChampionshipPrediction prediction)
    {
        var results = await repository.ReadResultsAsync(prediction.Season);
        if (!results.HasData || !results.IsOpen)
        {
            throw new ChampionshipPredictionsClosedException(prediction.Season);
        }

        await repository.CreateOrUpdateAsync(prediction);
    }

    public async Task<F1ChampionshipResults> ReadResultsAsync(int season)
    {
        return await repository.ReadResultsAsync(season);
    }

    public async Task WriteResultsAsync(int season, F1ChampionshipResults results)
    {
        await repository.WriteResultsAsync(season, results);
    }
}
