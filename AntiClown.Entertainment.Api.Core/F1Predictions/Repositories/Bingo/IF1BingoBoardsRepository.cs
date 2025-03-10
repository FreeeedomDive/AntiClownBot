namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public interface IF1BingoBoardsRepository
{
    Task<Guid[]> FindAsync(Guid userId, int season);
    Task CreateAsync(Guid userId, int season, Guid[] cards);
}