/* Generated file */
using RestSharp;
using AntiClown.Entertainment.Api.Client.Extensions;

namespace AntiClown.Entertainment.Api.Client.ActiveDailyEventsIndex;

public class ActiveDailyEventsIndexClient : IActiveDailyEventsIndexClient
{
    public ActiveDailyEventsIndexClient(RestSharp.RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async System.Threading.Tasks.Task<Dictionary<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto, System.Boolean>> ReadAllEventTypesAsync()
    {
        var request = new RestRequest("entertainmentApi/events/daily/index/", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<Dictionary<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto, System.Boolean>>();
    }

    public async System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto[]> ReadActiveEventsAsync()
    {
        var request = new RestRequest("entertainmentApi/events/daily/index/active", Method.Get);
        var response = await restClient.ExecuteAsync(request);
        return response.TryDeserialize<AntiClown.Entertainment.Api.Dto.DailyEvents.DailyEventTypeDto[]>();
    }

    public async System.Threading.Tasks.Task CreateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto)
    {
        var request = new RestRequest("entertainmentApi/events/daily/index/", Method.Post);
        request.AddJsonBody(activeDailyEventIndexDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task UpdateAsync(AntiClown.Entertainment.Api.Dto.DailyEvents.ActiveEventsIndex.ActiveDailyEventIndexDto activeDailyEventIndexDto)
    {
        var request = new RestRequest("entertainmentApi/events/daily/index/", Method.Put);
        request.AddJsonBody(activeDailyEventIndexDto);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async System.Threading.Tasks.Task ActualizeIndexAsync()
    {
        var request = new RestRequest("entertainmentApi/events/daily/index/actualize", Method.Post);
        var response = await restClient.ExecuteAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestSharp.RestClient restClient;
}
