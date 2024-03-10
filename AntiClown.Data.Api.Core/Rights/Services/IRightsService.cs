namespace AntiClown.Data.Api.Core.Rights.Services;

public interface IRightsService
{
    Task<Dictionary<Domain.Rights, Guid[]>> ReadAllAsync();
    Task<Domain.Rights[]> FindAllUserRightsAsync(Guid userId);
    Task GrantAsync(Guid userId, Domain.Rights right);
    Task RevokeAsync(Guid userId, Domain.Rights right);
}