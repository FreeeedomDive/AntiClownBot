using AntiClown.Data.Api.Core.Rights.Repositories;

namespace AntiClown.Data.Api.Core.Rights.Services;

public class RightsService : IRightsService
{
    public RightsService(IRightsRepository rightsRepository)
    {
        this.rightsRepository = rightsRepository;
    }

    public async Task<Dictionary<Domain.Rights, Guid[]>> ReadAllAsync()
    {
        var allRights = await rightsRepository.ReadAllAsync();
        return allRights
               .Select(x => new { x.UserId, Right = Enum.TryParse<Domain.Rights>(x.Name, out var rightName) ? rightName : (Domain.Rights?)null })
               .Where(x => x.Right is not null)
               .GroupBy(x => x.Right, x => x.UserId)
               .ToDictionary(x => x.Key!.Value, x => x.ToArray());
    }

    public async Task<Domain.Rights[]> FindAllUserRightsAsync(Guid userId)
    {
        return await rightsRepository.FindAsync(userId);
    }

    public async Task GrantAsync(Guid userId, Domain.Rights right)
    {
        var isGranted = await rightsRepository.ExistsAsync(userId, right);
        if (isGranted)
        {
            return;
        }

        await rightsRepository.CreateAsync(userId, right);
    }

    public async Task RevokeAsync(Guid userId, Domain.Rights right)
    {
        var isGranted = await rightsRepository.ExistsAsync(userId, right);
        if (!isGranted)
        {
            return;
        }

        await rightsRepository.DeleteAsync(userId, right);
    }

    private readonly IRightsRepository rightsRepository;
}