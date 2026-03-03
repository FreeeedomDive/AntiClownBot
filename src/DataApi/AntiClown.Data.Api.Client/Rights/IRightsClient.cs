/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Data.Api.Client.Rights;

public interface IRightsClient
{
    Task<Dictionary<AntiClown.Data.Api.Dto.Rights.RightsDto, System.Guid[]>> ReadAllAsync();
    Task<AntiClown.Data.Api.Dto.Rights.RightsDto[]> FindAllUserRightsAsync(System.Guid userId);
    Task GrantAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right);
    Task RevokeAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right);
}
