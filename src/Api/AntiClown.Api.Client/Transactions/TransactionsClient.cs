/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Api.Client.Transactions;

public class TransactionsClient : ITransactionsClient
{
    public TransactionsClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Api.Dto.Economies.TransactionDto[]> ReadTransactionsAsync(System.Guid userId, int skip = 0, int take = 10)
    {
        var requestBuilder = new RequestBuilder($"api/economy/{userId}/transactions", HttpRequestMethod.GET);
        requestBuilder.WithQueryParameter("skip", skip);
        requestBuilder.WithQueryParameter("take", take);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Economies.TransactionDto[]>(requestBuilder.Build());
    }

    public async Task<AntiClown.Api.Dto.Economies.TransactionDto[]> FindTransactionsAsync(AntiClown.Api.Dto.Economies.TransactionsFilterDto filter)
    {
        var requestBuilder = new RequestBuilder($"api/economy/transactions/find", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(filter);
        return await client.MakeRequestAsync<AntiClown.Api.Dto.Economies.TransactionDto[]>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
