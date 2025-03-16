using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.EventsProducing;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions.Bingo;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public class F1BingoCardsService(
    IF1BingoCardsRepository bingoCardsRepository,
    IF1BingoBoardsRepository bingoBoardsRepository,
    IF1PredictionsMessageProducer f1PredictionsMessageProducer,
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

    // keep it public for Hangfire
    // ReSharper disable once MemberCanBePrivate.Global
    public async Task CalculateBingosAsync(int season)
    {
        var boards = await bingoBoardsRepository.FindAsync(season);
        var cards = await bingoCardsRepository.FindAsync(season);
        var completedBoards = F1BingoCompletionsCalculator.GetCompletedBoards(boards, cards);
        var newCompletedBoards = completedBoards.ExceptBy(boards.Where(x => x.IsCompleted).Select(x => x.UserId), x => x.UserId).ToArray();
        foreach (var completedBoard in newCompletedBoards)
        {
            completedBoard.IsCompleted = true;
            await bingoBoardsRepository.UpdateAsync(completedBoard);
            await f1PredictionsMessageProducer.ProduceBingoCompletedAsync(completedBoard.UserId);
        }
    }
}