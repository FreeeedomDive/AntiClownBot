using AntiClown.Entertainment.Api.Client.Extensions;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.DailyEvents.PaymentsAndResets;

public class PaymentsAndResetsClient : IPaymentsAndResetsClient
{
    public PaymentsAndResetsClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<Guid> StartNewAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/start");
        var response = await restClient.ExecutePostAsync(request);
        return response.TryDeserialize<Guid>();
    }

    private readonly RestClient restClient;

    private const string ControllerUrl = "events/daily/paymentsAndResets";
}