/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.TransfusionEvent;

public class TransfusionEventClient : ITransfusionEventClient
{
    public TransfusionEventClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion.TransfusionEventDto> ReadAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/transfusion/{eventId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion.TransfusionEventDto>(requestBuilder.Build());
    }

    public async Task<System.Guid> StartNewAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/transfusion/start", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
