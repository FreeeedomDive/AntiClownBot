using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions.Bingo;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public class F1BingoCardsService(IF1BingoCardsRepository bingoCardsRepository) : IF1BingoCardsService
{
    public async Task<F1BingoCard[]> FindAsync(int season)
    {
        return await bingoCardsRepository.FindAsync(season);
    }

    public async Task<Guid> CreateCardAsync(CreateF1BingoCard createF1BingoCard)
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

    public async Task UpdateCardAsync(Guid id, UpdateF1BingoCard updateF1BingoCard)
    {
        var currentCard = await bingoCardsRepository.TryReadAsync(id);
        if (currentCard is null)
        {
            throw new F1BingoCardNotFoundException(id);
        }

        currentCard.CompletedRepeats = updateF1BingoCard.NewRepeatsCount;
        currentCard.IsCompleted = currentCard.CompletedRepeats == currentCard.TotalRepeats;
        await bingoCardsRepository.UpdateAsync(currentCard);
    }
}