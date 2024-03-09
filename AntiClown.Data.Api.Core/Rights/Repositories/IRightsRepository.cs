namespace AntiClown.Data.Api.Core.Rights.Repositories;

public interface IRightsRepository
{
    Task<Domain.Rights[]> FindAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId, Domain.Rights right);
    Task CreateAsync(Guid userId, Domain.Rights right);
    Task DeleteAsync(Guid userId, Domain.Rights right);
}