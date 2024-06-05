/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Data.Api.Client.Rights;

public class RightsClient : IRightsClient
{
    public RightsClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<Dictionary<AntiClown.Data.Api.Dto.Rights.RightsDto, System.Guid[]>> ReadAllAsync()
    {
        var request = new RestRequest("dataApi/rights/", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<Dictionary<AntiClown.Data.Api.Dto.Rights.RightsDto, System.Guid[]>>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Rights.RightsDto[]> FindAllUserRightsAsync(System.Guid userId)
    {
        var request = new RestRequest("dataApi/rights/{userId}", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Data.Api.Dto.Rights.RightsDto[]>();
    }

    public async System.Threading.Tasks.Task GrantAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right)
    {
        var request = new RestRequest("dataApi/rights/{userId}/grant", Method.Post);
        request.AddUrlSegment("userId", userId);
        request.AddQueryParameter("right", right.ToString());
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task RevokeAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right)
    {
        var request = new RestRequest("dataApi/rights/{userId}/revoke", Method.Delete);
        request.AddUrlSegment("userId", userId);
        request.AddQueryParameter("right", right.ToString());
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
