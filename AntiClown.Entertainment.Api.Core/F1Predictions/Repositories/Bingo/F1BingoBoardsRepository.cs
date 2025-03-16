using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public class F1BingoBoardsRepository(ISqlRepository<F1BingoBoardStorageElement> sqlRepository) : IF1BingoBoardsRepository
{
    public async Task<Guid[]> FindAsync(Guid userId, int season)
    {
        var result = await sqlRepository.FindAsync(x => x.UserId == userId && x.Season == season);
        return result.SelectMany(x => x.Cards).ToArray();
    }

    public async Task<F1BingoBoard[]> FindAsync(int season)
    {
        var result = await sqlRepository.FindAsync(x => x.Season == season);
        return result.Select(x => new F1BingoBoard
        {
            UserId = x.UserId,
            Cards = x.Cards,
        }).ToArray();
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
                IsCompleted = false,
            }
        );
    }

    public async Task UpdateAsync(F1BingoBoard board)
    {
        var result = (await sqlRepository.FindAsync(x => x.UserId == board.UserId && x.Season == board.Season)).FirstOrDefault();
        if (result is null)
        {
            return;
        }

        await sqlRepository.UpdateAsync(result.Id, x => x.IsCompleted = board.IsCompleted);
    }
}