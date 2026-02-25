using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.ChampionshipPredictions;

public class F1ChampionshipPredictionsRepository(ISqlRepository<F1ChampionshipPredictionStorageElement> sqlRepository) : IF1ChampionshipPredictionsRepository
{
    public async Task<F1ChampionshipPrediction> ReadAsync(Guid userId, int season)
    {
        var rows = await sqlRepository.FindAsync(x => x.UserId == userId && x.Season == season);
        return ToModel(userId, season, rows);
    }

    public async Task CreateOrUpdateAsync(F1ChampionshipPrediction prediction)
    {
        await UpsertRowAsync(prediction.UserId, prediction.Season, F1ChampionshipPredictionType.PreSeason, prediction.PreSeasonStandings);
        await UpsertRowAsync(prediction.UserId, prediction.Season, F1ChampionshipPredictionType.MidSeason, prediction.MidSeasonStandings);
    }

    private async Task UpsertRowAsync(Guid userId, int season, F1ChampionshipPredictionType type, string[]? standings)
    {
        if (standings is null)
        {
            return;
        }

        var existing = (await sqlRepository.FindAsync(
            x => x.UserId == userId && x.Season == season && x.Type == type.ToString()
        )).FirstOrDefault();

        if (existing is not null)
        {
            await sqlRepository.UpdateAsync(existing.Id, x => x.DriverStandings = standings);
        }
        else
        {
            await sqlRepository.CreateAsync(new F1ChampionshipPredictionStorageElement
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Season = season,
                Type = type.ToString(),
                DriverStandings = standings,
            });
        }
    }

    private static F1ChampionshipPrediction ToModel(Guid userId, int season, F1ChampionshipPredictionStorageElement[] rows) => new()
    {
        UserId = userId,
        Season = season,
        PreSeasonStandings = rows.FirstOrDefault(x => x.Type == nameof(F1ChampionshipPredictionType.PreSeason))?.DriverStandings,
        MidSeasonStandings = rows.FirstOrDefault(x => x.Type == nameof(F1ChampionshipPredictionType.MidSeason))?.DriverStandings,
    };
}
