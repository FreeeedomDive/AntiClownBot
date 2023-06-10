using AntiClown.Api.Client.Extensions;
using AntiClown.Api.Dto.Economies;
using RestSharp;

namespace AntiClown.Api.Client.Economy;

public class TransactionsClient : ITransactionsClient
{
    public TransactionsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<TransactionDto[]> ReadAsync(Guid userId, int skip = 0, int take = 10)
    {
        var request = new RestRequest(BuildApiUrl(userId)).AddQueryParameter("skip", skip).AddQueryParameter("take", take);
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<TransactionDto[]>();
    }

    private static string BuildApiUrl(Guid userId) => $"economy/{userId}/transactions";

    private readonly RestClient restClient;
}