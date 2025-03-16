using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions.Bingo;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public class F1BingoCardsService(
    IF1BingoCardsRepository bingoCardsRepository,
    IF1BingoBoardsRepository bingoBoardsRepository,
    IScheduler scheduler
) : IF1BingoCardsService
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
        ScheduleBoardsBingoCalculations(currentCard.Season);
    }

    private void ScheduleBoardsBingoCalculations(int season)
    {
        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => CalculateBingosAsync(season),
                TimeSpan.FromSeconds(1)
            )
        );
    }

    public async Task CalculateBingosAsync(int season)
    {
        var bingoCompletions = new[]
        {
            // horizontal
            new[] { 0, 1, 2, 3, 4 },
            new[] { 5, 6, 7, 8, 9 },
            new[] { 10, 11, 12, 13, 14 },
            new[] { 15, 16, 17, 18, 19 },
            new[] { 20, 21, 22, 23, 24 },
            // vertical
            new[] { 0, 5, 10, 15, 20 },
            new[] { 1, 6, 11, 16, 21 },
            new[] { 2, 7, 12, 17, 22 },
            new[] { 3, 8, 13, 18, 23 },
            new[] { 4, 9, 14, 19, 24 },
            // diagonal
            new[] { 0, 6, 12, 18, 24 },
            new[] { 4, 8, 12, 16, 20 },
        };

        var boards = await bingoBoardsRepository.FindAsync(season);
        var cards = await bingoCardsRepository.FindAsync(season);
        var completedCards = cards.Where(x => x.IsCompleted).Select(x => x.Id).ToHashSet();
        foreach (var board in boards)
        {
            var userCompletedCardsIndices = board.Cards
                                                 .Select((x, i) => completedCards.Contains(x) ? i : (int?)null)
                                                 .Where(x => x is not null)
                                                 .Select(x => x!.Value)
                                                 .ToArray();
            var hasFullIntersects = bingoCompletions.Any(x => x.Intersect(userCompletedCardsIndices).Count() == 5);
        }
    }
}