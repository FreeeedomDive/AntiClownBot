using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Data.Api.Core.Rights.Repositories;

public class RightsRepository : IRightsRepository
{
    public RightsRepository(ISqlRepository<RightsStorageElement> sqlRepository)
    {
        this.sqlRepository = sqlRepository;
    }

    public async Task<RightsStorageElement[]> ReadAllAsync()
    {
        return await sqlRepository.ReadAllAsync();
    }

    public async Task<Domain.Rights[]> FindAsync(Guid userId)
    {
        var result = await sqlRepository.FindAsync(x => x.UserId == userId);
        var rights = result
                     .Select(x => Enum.TryParse<Domain.Rights>(x.Name, out var rightName) ? rightName : (Domain.Rights?)null)
                     .Where(x => x is not null)
                     .Select(x => x!.Value)
                     .ToArray();
        return rights;
    }

    public async Task<bool> ExistsAsync(Guid userId, Domain.Rights right)
    {
        var rightName = right.ToString();
        var existing = await sqlRepository.FindAsync(x => x.UserId == userId && x.Name == rightName);
        return existing.Length > 0;
    }

    public async Task CreateAsync(Guid userId, Domain.Rights right)
    {
        await sqlRepository.CreateAsync(
            new RightsStorageElement
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = right.ToString(),
            }
        );
    }

    public async Task DeleteAsync(Guid userId, Domain.Rights right)
    {
        var rightName = right.ToString();
        var existing = await sqlRepository.FindAsync(x => x.UserId == userId && x.Name == rightName);
        await sqlRepository.DeleteAsync(existing);
    }

    private readonly ISqlRepository<RightsStorageElement> sqlRepository;
}