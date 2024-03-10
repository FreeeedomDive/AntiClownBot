using AntiClown.Core.Dto.Extensions;
using AntiClown.Data.Api.Dto.Rights;
using RestSharp;
using RestSharpClient.Extensions;

namespace AntiClown.Data.Api.Client.Rights;

public class RightsClient : IRightsClient
{
    public RightsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<Dictionary<RightsDto, Guid[]>> ReadAllAsync()
    {
        var request = new RestRequest("rights");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<Dictionary<RightsDto, Guid[]>>();
    }

    public async Task<RightsDto[]> FindAllUserRightsAsync(Guid userId)
    {
        var request = new RestRequest($"rights/{userId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<RightsDto[]>();
    }

    public async Task GrantAsync(Guid userId, RightsDto right)
    {
        var request = new RestRequest($"rights/{userId}/grant").AddQueryParameter(nameof(right), right);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task RevokeAsync(Guid userId, RightsDto right)
    {
        var request = new RestRequest($"rights/{userId}/revoke").AddQueryParameter(nameof(right), right);
        var response = await restClient.ExecuteDeleteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
}