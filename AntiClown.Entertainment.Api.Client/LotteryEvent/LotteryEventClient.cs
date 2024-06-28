/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.LotteryEvent;

public class LotteryEventClient : ILotteryEventClient
{
    public LotteryEventClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery.LotteryEventDto> ReadAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/lottery/{eventId}", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery.LotteryEventDto>(requestBuilder.Build());
    }

    public async Task<System.Guid> StartNewAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/lottery/start", HttpRequestMethod.POST);
        return await client.MakeRequestAsync<System.Guid>(requestBuilder.Build());
    }

    public async Task AddParticipantAsync(System.Guid eventId, System.Guid userId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/lottery/{eventId}/addParticipant", HttpRequestMethod.POST);
        requestBuilder.WithQueryParameter("userId", userId);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task FinishAsync(System.Guid eventId)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/lottery/{eventId}/finish", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
