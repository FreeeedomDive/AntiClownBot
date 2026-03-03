/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Data.Api.Client.Tokens;

public class TokensClient : ITokensClient
{
    public TokensClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task InvalidateAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"dataApi/tokens/{userId}/", HttpRequestMethod.DELETE);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task ValidateAsync(System.Guid userId, AntiClown.Data.Api.Dto.Tokens.TokenDto token)
    {
        var requestBuilder = new RequestBuilder($"dataApi/tokens/{userId}/validate", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(token);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task<string> GetAsync(System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"dataApi/tokens/{userId}/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<string>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
