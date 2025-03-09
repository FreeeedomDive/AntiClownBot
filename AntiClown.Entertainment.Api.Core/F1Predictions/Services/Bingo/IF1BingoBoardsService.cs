namespace AntiClown.Entertainment.Api.Core.F1Predictions.Services.Bingo;

public interface IF1BingoBoardsService
{
    Task<Guid[]> GetOrCreateBingoBoard(Guid userId);
}