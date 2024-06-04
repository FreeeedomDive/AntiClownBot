/* Generated file */
using RestSharp;
using AntiClown.Api.Client.Extensions;

namespace AntiClown.Api.Client.Economy;

public class EconomyClient : IEconomyClient
{
    public EconomyClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.EconomyDto> ReadAsync(System.Guid userId)
    {
        var request = new RestRequest("api/economy/{userId}", Method.Get);
        request.AddUrlSegment("userId", userId);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Economies.EconomyDto>();
    }

    public async System.Threading.Tasks.Task UpdateScamCoinsAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateScamCoinsDto updateScamCoinsDto)
    {
        var request = new RestRequest("api/economy/{userId}/scamCoins", Method.Post);
        request.AddUrlSegment("userId", userId);
        request.AddJsonBody(updateScamCoinsDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task UpdateScamCoinsForAllAsync(System.Int32 scamCoinsDiff, System.String reason)
    {
        var request = new RestRequest("api/economy/scamCoins/updateForAll", Method.Post);
        request.AddQueryParameter("scamCoinsDiff", scamCoinsDiff.ToString());
        request.AddQueryParameter("reason", reason.ToString());
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task UpdateLootBoxesAsync(System.Guid userId, AntiClown.Api.Dto.Economies.UpdateLootBoxesDto lootBoxesDto)
    {
        var request = new RestRequest("api/economy/{userId}/lootBoxes", Method.Post);
        request.AddUrlSegment("userId", userId);
        request.AddJsonBody(lootBoxesDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task ResetAllCoolDownsAsync()
    {
        var request = new RestRequest("api/economy/resetAllCoolDowns", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
