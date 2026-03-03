/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.PaymentsAndResetsEvent;

public class PaymentsAndResetsEventClient : IPaymentsAndResetsEventClient
{
    public PaymentsAndResetsEventClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<System.Guid> StartNewAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/paymentsAndResets/start", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
