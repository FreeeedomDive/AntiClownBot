/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.ActiveEventsIndex;

public class ActiveEventsIndexClient : IActiveEventsIndexClient
{
    public ActiveEventsIndexClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<Dictionary<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto, System.Boolean>> ReadAllEventTypesAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/index/", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<Dictionary<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto, System.Boolean>>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto[]> ReadActiveEventsAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/index/active", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.CommonEvents.CommonEventTypeDto[]>();
    }

    public async System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto)
    {
        var request = new RestRequest("entertainmentApi/events/common/index/", Method.Post);
        request.AddJsonBody(activeCommonEventIndexDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task UpdateAsync(AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex.ActiveCommonEventIndexDto activeCommonEventIndexDto)
    {
        var request = new RestRequest("entertainmentApi/events/common/index/", Method.Put);
        request.AddJsonBody(activeCommonEventIndexDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task ActualizeIndexAsync()
    {
        var request = new RestRequest("entertainmentApi/events/common/index/actualize", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
