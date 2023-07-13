using AntiClown.EntertainmentApi.Client.Extensions;
using AntiClown.EntertainmentApi.Dto.DailyEvents;
using AntiClown.EntertainmentApi.Dto.DailyEvents.ActiveEventsIndex;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.DailyEvents.ActiveEventsIndex;

public class ActiveDailyEventsIndexClient : IActiveDailyEventsIndexClient
{
    public ActiveDailyEventsIndexClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<Dictionary<DailyEventTypeDto, bool>> ReadAllEventTypesAsync()
    {
        var request = new RestRequest(ControllerUrl);
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<Dictionary<DailyEventTypeDto, bool>>();
    }

    public async Task<DailyEventTypeDto[]> ReadActiveEventsAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/active");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<DailyEventTypeDto[]>();
    }

    public async Task CreateAsync(DailyEventTypeDto eventType, bool isActiveByDefault)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(new ActiveDailyEventIndexDto
        {
            EventType = eventType,
            IsActive = isActiveByDefault
        });
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task UpdateAsync(DailyEventTypeDto eventType, bool isActive)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(new ActiveDailyEventIndexDto
        {
            EventType = eventType,
            IsActive = isActive
        });
        var response = await restClient.ExecutePutAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task ActualizeIndexAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/actualize");
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;
    private const string ControllerUrl = "events/daily/index";
}