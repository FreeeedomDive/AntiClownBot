/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Api.Client.Transactions;

public class TransactionsClient : ITransactionsClient
{
    public TransactionsClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.TransactionDto[]> ReadTransactionsAsync(System.Guid userId, System.Int32 skip = 0, System.Int32 take = 10)
    {
        var request = new RestRequest("api/economy/{userId}/transactions", Method.Get);
        request.AddUrlSegment("userId", userId);
        request.AddQueryParameter("skip", skip.ToString());
        request.AddQueryParameter("take", take.ToString());
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Economies.TransactionDto[]>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.TransactionDto[]> FindTransactionsAsync(AntiClown.Api.Dto.Economies.TransactionsFilterDto filter)
    {
        var request = new RestRequest("api/economy/transactions/find", Method.Post);
        request.AddJsonBody(filter);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Api.Dto.Economies.TransactionDto[]>();
    }

    private readonly RestSharp.RestClient restClient;
}
