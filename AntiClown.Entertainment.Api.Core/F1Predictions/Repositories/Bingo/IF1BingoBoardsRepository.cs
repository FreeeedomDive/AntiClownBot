namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public interface IF1BingoBoardsRepository
{
    Task<Guid[]> TryReadAsync(Guid userId);
    Task CreateAsync(Guid userId, Guid[] cards);
}