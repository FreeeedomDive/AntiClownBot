using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public interface IF1BingoCardsRepository
{
    Task<F1BingoCard[]> FindAsync(int season);
    Task CreateAsync(F1BingoCard bingoCard);
    Task UpdateAsync(F1BingoCard bingoCard);
}