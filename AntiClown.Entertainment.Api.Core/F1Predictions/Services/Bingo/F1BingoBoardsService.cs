using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public class F1BingoBoardsService(
    IF1BingoBoardsRepository bingoBoardsRepository,
    IF1BingoCardsRepository bingoCardsRepository
) : IF1BingoBoardsService
{
    public async Task<Guid[]> GetOrCreateBingoBoard(Guid userId)
    {
        var existing = await bingoBoardsRepository.TryReadAsync(userId);
        if (existing.Length != 0)
        {
            return existing;
        }

        var allCards = await bingoCardsRepository.FindAsync(DateTime.UtcNow.Year);
        var userCards = allCards.Select(x => x.Id).Shuffle().ToArray();
        await bingoBoardsRepository.CreateAsync(userId, userCards);
        return userCards;
    }
}