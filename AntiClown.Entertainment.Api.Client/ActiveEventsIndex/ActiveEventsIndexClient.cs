/* Generated file */
using System.Threading.Tasks;

using Xdd.HttpHelpers.Models.Extensions;
using Xdd.HttpHelpers.Models.Requests;

namespace AntiClown.Entertainment.Api.Client.ActiveEventsIndex;

public class ActiveEventsIndexClient : IActiveEventsIndexClient
{
    public ActiveEventsIndexClient(RestSharp.RestClient client)
    {
        this.client = client;
    }

    public async Task<Dictionary<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto, bool>> ReadAllEventTypesAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/index/", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<Dictionary<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto, bool>>(requestBuilder.Build());
    }

    public async Task<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto[]> ReadActiveEventsAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/index/active", HttpRequestMethod.GET);
        return await client.MakeRequestAsync<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto[]>(requestBuilder.Build());
    }

    public async Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/index/", HttpRequestMethod.POST);
        requestBuilder.WithJsonBody(activeCommonEventIndexDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto)
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/index/", HttpRequestMethod.PUT);
        requestBuilder.WithJsonBody(activeCommonEventIndexDto);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    public async Task ActualizeIndexAsync()
    {
        var requestBuilder = new RequestBuilder($"entertainmentApi/events/common/index/actualize", HttpRequestMethod.POST);
        await client.MakeRequestAsync(requestBuilder.Build());
    }

    private readonly RestSharp.RestClient client;
}
