/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.Parties;

public class PartiesClient : IPartiesClient
{
    public PartiesClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto> ReadAsync(System.Guid id)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/parties/{id}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.Parties.PartyDto>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadOpenedAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/parties/opened", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadFullAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/parties/full", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]>(requestBuilder.Build());
    }

    public async Task<System.Guid> CreateAsync(AntiClown.Entertainment.Api.Dto.Parties.CreatePartyDto newParty)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/parties/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(newParty);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    public async Task JoinAsync(System.Guid id, System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/parties/{id}/players/{userId}/join", HttpRequestMethod.PUT);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task LeaveAsync(System.Guid id, System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/parties/{id}/players/{userId}/leave", HttpRequestMethod.PUT);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task CloseAsync(System.Guid id)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/parties/{id}/close", HttpRequestMethod.PUT);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
