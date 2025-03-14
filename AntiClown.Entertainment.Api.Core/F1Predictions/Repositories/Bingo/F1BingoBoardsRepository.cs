﻿using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public class F1BingoBoardsRepository(ISqlRepository<F1BingoBoardStorageElement> sqlRepository) : IF1BingoBoardsRepository
{
    public async Task<Guid[]> FindAsync(Guid userId, int season)
    {
        var result = await sqlRepository.FindAsync(x => x.UserId == userId && x.Season == season);
        return result.SelectMany(x => x.Cards).ToArray();
    }

    public async Task CreateAsync(Guid userId, int season, Guid[] cards)
    {
        await sqlRepository.CreateAsync(
            new F1BingoBoardStorageElement
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Season = season,
                Cards = cards,
            }
        );
    }
}