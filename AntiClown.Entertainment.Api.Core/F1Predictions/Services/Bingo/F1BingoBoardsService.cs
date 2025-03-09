using AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public class F1BingoBoardsService(
    IF1BingoBoardsRepository bingoBoardsRepository,
    IF1BingoCardsRepository bingoCardsRepository
) : IF1BingoBoardsService
{
    public async Task<Guid[]> GetOrCreateBingoBoard(Guid userId, int season)
    {
        var existing = await bingoBoardsRepository.FindAsync(userId, season);
        if (existing.Length != 0)
        {
            return existing;
        }

        var allCards = await bingoCardsRepository.FindAsync(season);
        var userCards = allCards.Select(x => x.Id).Shuffle().ToArray();
        await bingoBoardsRepository.CreateAsync(userId, season, userCards);
        return userCards;
    }
}