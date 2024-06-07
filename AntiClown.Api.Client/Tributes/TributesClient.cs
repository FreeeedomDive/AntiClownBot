/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Api.Client.Tributes;

public class TributesClient : ITributesClient
{
    public TributesClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.NextTributeDto> WhenNextTributeAsync(System.Guid userId)
    {
        var request = new RestRequest("api/economy/{userId}/tributes/when", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Economies.NextTributeDto>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.TributeDto> TributeAsync(System.Guid userId)
    {
        var request = new RestRequest("api/economy/{userId}/tributes/", Method.Post);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Economies.TributeDto>();
    }

    private readonly RestSharp.RestClient restClient;
}
