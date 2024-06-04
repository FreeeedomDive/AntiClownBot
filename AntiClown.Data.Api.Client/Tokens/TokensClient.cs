/* Generated file */
using RestSharp;
using AntiClown.Data.Api.Client.Extensions;

namespace AntiClown.Data.Api.Client.Tokens;

public class TokensClient : ITokensClient
{
    public TokensClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task InvalidateAsync(System.Guid userId)
    {
        var request = new RestRequest("dataApi/tokens/{userId}/", Method.Delete);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task ValidateAsync(System.Guid userId, AntiClown.Data.Api.Dto.Tokens.TokenDto token)
    {
        var request = new RestRequest("dataApi/tokens/{userId}/validate", Method.Post);
        request.AddUrlSegment("userId", userId);
        request.AddJsonBody(token);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task<System.String> GetAsync(System.Guid userId)
    {
        var request = new RestRequest("dataApi/tokens/{userId}/", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.String>();
    }

    private readonly RestSharp.RestClient restClient;
}
