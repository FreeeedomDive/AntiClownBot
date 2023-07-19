using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.Parties;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.Parties;

public class PartiesClient : IPartiesClient
{
    public PartiesClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<PartyDto> ReadAsync(Guid id)
    {
        var request = new RestRequest($"{ControllerUrl}/{id}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<PartyDto>();
    }

    public async Task<PartyDto[]> ReadOpenedAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/opened");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<PartyDto[]>();
    }

    public async Task<Guid> CreateAsync(CreatePartyDto newParty)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(newParty);
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    public async Task UpdateAsync(PartyDto party)
    {
        var request = new RestRequest($"{ControllerUrl}/{party.Id}");
        request.AddJsonBody(party);
        var response = await restClient.ExecutePutAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task CloseAsync(Guid id)
    {
        var request = new RestRequest($"{ControllerUrl}/{id}/close");
        var response = await restClient.ExecutePutAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
    private const string ControllerUrl = "parties";
}