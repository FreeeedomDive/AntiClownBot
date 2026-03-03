/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Data.Api.Client.Rights;

public class RightsClient : IRightsClient
{
    public RightsClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<Dictionary<AntiClown.Data.Api.Dto.Rights.RightsDto, System.Guid[]>> ReadAllAsync()
    {
        var requestBuilder = new RequestBuilder($"dataApi/rights/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<Dictionary<AntiClown.Data.Api.Dto.Rights.RightsDto, System.Guid[]>>(requestBuilder.Build());
    }

    public async Task<AntiClown.Data.Api.Dto.Rights.RightsDto[]> FindAllUserRightsAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"dataApi/rights/{userId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Data.Api.Dto.Rights.RightsDto[]>(requestBuilder.Build());
    }

    public async Task GrantAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right)
    {
        var requestBuilder = new RequestBuilder($"dataApi/rights/{userId}/grant", HttpRequestMethod.POST);
        requestBuilder.WithQueryParameter("right", right);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task RevokeAsync(System.Guid userId, AntiClown.Data.Api.Dto.Rights.RightsDto right)
    {
        var requestBuilder = new RequestBuilder($"dataApi/rights/{userId}/revoke", HttpRequestMethod.DELETE);
        requestBuilder.WithQueryParameter("right", right);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
