/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.ActiveDailyEventsIndex;

public class ActiveDailyEventsIndexClient : IActiveDailyEventsIndexClient
{
    public ActiveDailyEventsIndexClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<Dictionary<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto, bool>> ReadAllEventTypesAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/index/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<Dictionary<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto, bool>>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto[]> ReadActiveEventsAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/index/active", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto[]>(requestBuilder.Build());
    }

    public async Task CreateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/index/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(activeDailyEventIndexDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task UpdateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/index/", HttpRequestMethod.PUT);
        requestBuilder.WithJsonBody(activeDailyEventIndexDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task ActualizeIndexAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/daily/index/actualize", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
