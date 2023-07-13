using AntiClown.EntertainmentApi.Client.Extensions;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.DailyEvents.PaymentsAndResets;

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

    private const string ControllerUrl = "events/daily/paymentsAndResets";
    private readonly RestClient restClient;
}