/* Generated file */
namespace AntiClown.Data.Api.Client.Rights;

public interface IRightsClient
{
    System.Threading.Tasks.Task<Dictionary<AntiClown.Data.Api.Dto.Rights.RightsDto, System.Guid[]>> ReadAllAsync();
    System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Rights.RightsDto[]> FindAllUserRightsAsync(System.Guid userId);
    System.Threading.Tasks.Task GrantAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right);
    System.Threading.Tasks.Task RevokeAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right);
}
