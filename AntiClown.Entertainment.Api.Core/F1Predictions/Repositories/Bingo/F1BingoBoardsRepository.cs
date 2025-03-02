using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.Repositories.Bingo;

public class F1BingoBoardsRepository(ISqlRepository<F1BingoBoardStorageElement> sqlRepository) : IF1BingoBoardsRepository
{
    public async Task<Guid[]> TryReadAsync(Guid userId)
    {
        var result = await sqlRepository.TryReadAsync(userId);
        return result?.Cards ?? [];
    }

    public async Task CreateAsync(Guid userId, Guid[] cards)
    {
        await sqlRepository.CreateAsync(
            new F1BingoBoardStorageElement
            {
                Id = userId,
                Cards = cards,
            }
        );
    }
}