using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public interface IF1BingoCardsService
{
    Task<F1BingoCard[]> FindAsync(int season);
    Task<Guid> CreateCardAsync(CreateF1BingoCard createF1BingoCard);
    Task UpdateCardAsync(Guid id, UpdateF1BingoCard updateF1BingoCard);
}