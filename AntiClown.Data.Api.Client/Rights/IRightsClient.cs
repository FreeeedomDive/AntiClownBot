using AntiClown.Data.Api.Dto.Rights;

namespace AntiClown.Data.Api.Client.Rights;

public interface IRightsClient
{
    Task<Dictionary<RightsDto, Guid[]>> ReadAllAsync();
    Task<RightsDto[]> FindAllUserRightsAsync(Guid userId);
    Task GrantAsync(Guid userId);
    Task RevokeAsync(Guid userId);
}