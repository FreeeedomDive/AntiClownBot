using AntiClown.Data.Api.Core.Rights.Repositories;

namespace AntiClown.Data.Api.Core.Rights.Services;

public class RightsService : IRightsService
{
    public RightsService(IRightsRepository rightsRepository)
    {
        this.rightsRepository = rightsRepository;
    }

    public async Task<Domain.Rights[]> FindAllUserRights(Guid userId)
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