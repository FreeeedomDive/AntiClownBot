namespace AntiClown.Api.Core.Users.Repositories;

public interface IUserIntegrationsRepository
{
    Task CreateOrUpdateAsync(Guid userId, string integrationName, string integrationUserId);
    Task<Guid?> FindAsync(string integrationName, string integrationUserId);
}