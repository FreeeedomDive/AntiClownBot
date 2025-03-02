using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public class F1BingoCardsService(IF1BingoCardsRepository bingoCardsRepository) : IF1BingoCardsService
{
    public async Task<Guid> CreateBingoCardAsync(CreateF1BingoCard createF1BingoCard)
    {
        var card = new F1BingoCard
        {
            Id = Guid.NewGuid(),
            Season = createF1BingoCard.Season,
            Description = createF1BingoCard.Description,
            Explanation = createF1BingoCard.Explanation,
            Probability = createF1BingoCard.Probability,
            TotalRepeats = createF1BingoCard.TotalRepeats,
            CompletedRepeats = 0,
            IsCompleted = false,
        };

        await bingoCardsRepository.CreateAsync(card);
        return card.Id;
    }
}