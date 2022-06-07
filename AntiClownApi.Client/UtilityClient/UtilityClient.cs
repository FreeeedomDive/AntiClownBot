using AntiClownApiClient.Dto.Responses.UserCommandResponses;
using AntiClownApiClient.Extensions;
using RestSharp;

namespace AntiClownApiClient.UtilityClient;

public class UtilityClient : IUtilityClient
{
    public UtilityClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<List<TributeResponseDto>> GetAutomaticTributesAsync()
    {
        var request = new RestRequest($"api/globalState/autoTributes");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<List<TributeResponseDto>>();
    }

    public async Task<bool> PingApiAsync()
    {
        var request = new RestRequest($"api/globalState/ping");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<string>() == "OK";
    }

    private readonly RestClient restClient;
}