namespace AntiClown.Data.Api.Core.Rights.Services;

public interface IRightsService
{
    Task<Domain.Rights[]> FindAllUserRights(Guid userId);
    Task GrantAsync(Guid userId, Domain.Rights right);
    Task RevokeAsync(Guid userId, Domain.Rights right);
}