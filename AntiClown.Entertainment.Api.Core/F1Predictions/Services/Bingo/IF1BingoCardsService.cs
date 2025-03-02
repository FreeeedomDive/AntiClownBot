using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public interface IF1BingoCardsService
{
    Task<Guid> CreateBingoCardAsync(CreateF1BingoCard createF1BingoCard);
}