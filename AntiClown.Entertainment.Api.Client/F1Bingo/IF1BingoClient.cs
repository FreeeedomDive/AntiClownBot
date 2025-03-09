/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.F1Bingo;

public interface IF1BingoClient
{
    Task<AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo.F1BingoCardDto[]> ReadCardsAsync(int season);
    Task CreateCardAsync(AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo.CreateF1BingoCardDto dto);
    Task UpdateCardAsync(System.Guid cardId, AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo.F1BingoCardDto dto);
    Task<System.Guid[]> GetBoardAsync(System.Guid userId, int season);
}
