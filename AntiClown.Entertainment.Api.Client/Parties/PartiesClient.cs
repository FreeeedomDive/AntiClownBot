/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.Parties;

public class PartiesClient : IPartiesClient
{
    public PartiesClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto> ReadAsync(System.Guid id)
    {
        var request = new RestRequest("entertainmentApi/parties/{id}", Method.Get);
        request.AddUrlSegment("id", id);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.Parties.PartyDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadOpenedAsync()
    {
        var request = new RestRequest("entertainmentApi/parties/opened", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadFullAsync()
    {
        var request = new RestRequest("entertainmentApi/parties/full", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]>();
    }

    public async System.Threading.Tasks.Task<System.Guid> CreateAsync(AntiClown.Entertainment.Api.Dto.Parties.CreatePartyDto newParty)
    {
        var request = new RestRequest("entertainmentApi/parties/", Method.Post);
        request.AddJsonBody(newParty);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    public async System.Threading.Tasks.Task JoinAsync(System.Guid id, System.Guid userId)
    {
        var request = new RestRequest("entertainmentApi/parties/{id}/players/{userId}/join", Method.Put);
        request.AddUrlSegment("id", id);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task LeaveAsync(System.Guid id, System.Guid userId)
    {
        var request = new RestRequest("entertainmentApi/parties/{id}/players/{userId}/leave", Method.Put);
        request.AddUrlSegment("id", id);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task CloseAsync(System.Guid id)
    {
        var request = new RestRequest("entertainmentApi/parties/{id}/close", Method.Put);
        request.AddUrlSegment("id", id);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
