/* Generated file */
using RestSharp;
using Xdd.HttpHelpers.Models.Extensions;

namespace AntiClown.Entertainment.Api.Client.PaymentsAndResetsEvent;

public class PaymentsAndResetsEventClient : IPaymentsAndResetsEventClient
{
    public PaymentsAndResetsEventClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<System.Guid> StartNewAsync()
    {
        var request = new RestRequest("entertainmentApi/events/daily/paymentsAndResets/start", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<System.Guid>();
    }

    private readonly RestSharp.RestClient restClient;
}
