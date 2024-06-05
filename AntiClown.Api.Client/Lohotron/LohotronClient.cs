/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Api.Client.Lohotron;

public class LohotronClient : ILohotronClient
{
    public LohotronClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.LohotronRewardDto> UseLohotronAsync(System.Guid userId)
    {
        var request = new RestRequest("api/economy/{userId}/lohotron", Method.Post);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Economies.LohotronRewardDto>();
    }

    public async System.Threading.Tasks.Task ResetAsync()
    {
        var request = new RestRequest("api/economy/lohotron/reset", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
