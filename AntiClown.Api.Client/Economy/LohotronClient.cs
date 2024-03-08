using AntiClown.Api.Dto.Economies;
using AntiClown.Core.Dto.Extensions;
using RestSharp;

namespace AntiClown.Api.Client.Economy;

public class LohotronClient : ILohotronClient
{
    public LohotronClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<LohotronRewardDto> UseLohotronAsync(Guid userId)
    {
        var request = new RestRequest($"economy/{userId}/lohotron");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<LohotronRewardDto>();
    }

    public async Task ResetLohotronForAllUsersAsync()
    {
        var request = new RestRequest($"economy/lohotron/reset");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
}