using AntiClown.Api.Dto.Economies;
using AntiClown.Core.Dto.Extensions;
using RestSharp;

namespace AntiClown.Api.Client.Economy;

public class TributeClient : ITributeClient
{
    public TributeClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<NextTributeDto> ReadNextTributeInfoAsync(Guid userId)
    {
        var request = new RestRequest($"{BuildApiUrl(userId)}/when");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<NextTributeDto>();
    }

    public async Task<TributeDto> TributeAsync(Guid userId)
    {
        var request = new RestRequest(BuildApiUrl(userId));
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<TributeDto>();
    }

    private static string BuildApiUrl(Guid userId) => $"economy/{userId}/tributes";

    private readonly RestClient restClient;
}