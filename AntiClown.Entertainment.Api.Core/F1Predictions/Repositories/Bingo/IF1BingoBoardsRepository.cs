using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public interface IF1BingoBoardsRepository
{
    Task<Guid[]> FindAsync(Guid userId, int season);
    Task<F1BingoBoard[]> FindAsync(int season);
    Task CreateAsync(Guid userId, int season, Guid[] cards);
    Task UpdateAsync(F1BingoBoard board);
}