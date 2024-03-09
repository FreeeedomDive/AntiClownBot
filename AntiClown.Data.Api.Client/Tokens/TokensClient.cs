using AntiClown.Core.Dto.Extensions;
using AntiClown.Data.Api.Dto.Tokens;
using RestSharpClient.Extensions;
using RestSharp;

namespace AntiClown.Data.Api.Client.Tokens;

public class TokensClient : ITokensClient
{
    public TokensClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task InvalidateAsync(Guid userId)
    {
        var request = new RestRequest($"tokens/{userId}");
        var response = await restClient.ExecuteDeleteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task ValidateAsync(Guid userId, string token)
    {
        var request = new RestRequest($"tokens/{userId}/validate").AddJsonBody(new TokenDto { Token = token });
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task<string> GetAsync(Guid userId)
    {
        var request = new RestRequest($"tokens/{userId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<string>();
    }

    private readonly RestClient restClient;
}