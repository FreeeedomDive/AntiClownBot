using AntiClown.Tools.Utility.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Users.Repositories;

public class UserIntegrationsRepository(ISqlRepository<UserIntegrationStorageElement> sqlRepository) : IUserIntegrationsRepository
{
    public async Task CreateOrUpdateAsync(Guid userId, string integrationName, string integrationUserId)
    {
        var integrationId = GetId(userId, integrationName);
        var existing = await sqlRepository.TryReadAsync(integrationId) is not null;
        var task = existing
            ? sqlRepository.UpdateAsync(integrationId, x => x.IntegrationUserId = integrationUserId)
            : sqlRepository.CreateAsync(
                new UserIntegrationStorageElement
                {
                    Id = integrationId,
                    UserId = userId,
                    IntegrationName = integrationName,
                    IntegrationUserId = integrationUserId,
                }
            );
        await task;
    }

    public async Task<Guid?> FindAsync(string integrationName, string integrationUserId)
    {
        var result = await sqlRepository.FindAsync(x => x.IntegrationName == integrationName && x.IntegrationUserId == integrationUserId);
        return result.FirstOrDefault()?.UserId;
    }

    private static Guid GetId(Guid userId, string integrationName)
    {
        return $"{userId}_{integrationName}".GetDeterministicGuid();
    }
}