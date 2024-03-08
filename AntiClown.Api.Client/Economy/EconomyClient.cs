using AntiClown.Api.Dto.Economies;
using AntiClown.Core.Dto.Extensions;
using RestSharp;

namespace AntiClown.Api.Client.Economy;

public class EconomyClient : IEconomyClient
{
    public EconomyClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<EconomyDto> ReadAsync(Guid userId)
    {
        var request = new RestRequest($"economy/{userId}");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<EconomyDto>();
    }

    public async Task UpdateScamCoinsAsync(Guid userId, int scamCoinsDiff, string reason)
    {
        var request = new RestRequest($"economy/{userId}/scamCoins");
        request.AddJsonBody(new UpdateScamCoinsDto
        {
            UserId = userId,
            ScamCoinsDiff = scamCoinsDiff,
            Reason = reason,
        });
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task UpdateScamCoinsForAllAsync(int scamCoinsDiff, string reason)
    {
        var request = new RestRequest($"economy/scamCoins/updateForAll")
            .AddQueryParameter("scamCoinsDiff", scamCoinsDiff)
            .AddQueryParameter("reason", reason);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task UpdateLootBoxesAsync(Guid userId, int lootBoxesDiff)
    {
        var request = new RestRequest($"economy/{userId}/lootBoxes");
        request.AddJsonBody(new UpdateLootBoxesDto
        {
            UserId = userId,
            LootBoxesDiff = lootBoxesDiff,
        });
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task ResetAllCoolDownsAsync()
    {
        var request = new RestRequest($"economy/resetAllCoolDowns");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
}