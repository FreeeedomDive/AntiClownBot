using AntiClown.Core.Dto.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents;
using AntiClown.Entertainment.Api.Dto.CommonEvents.ActiveEventsIndex;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.ActiveEventsIndex;

public class ActiveCommonEventsIndexClient : IActiveCommonEventsIndexClient
{
    public ActiveCommonEventsIndexClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<Dictionary<CommonEventTypeDto, bool>> ReadAllEventTypesAsync()
    {
        var request = new RestRequest(ControllerUrl);
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<Dictionary<CommonEventTypeDto, bool>>();
    }

    public async Task<CommonEventTypeDto[]> ReadActiveEventsAsync()
    {
        var request = new RestRequest($"{ControllerUrl}/active");
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<CommonEventTypeDto[]>();
    }

    public async Task CreateAsync(CommonEventTypeDto eventType, bool isActiveByDefault)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(
            new ActiveCommonEventIndexDto
            {
                EventType = eventType,
                IsActive = isActiveByDefault,
            }
        );
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task UpdateAsync(CommonEventTypeDto eventType, bool isActive)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(
            new ActiveCommonEventIndexDto
            {
                EventType = eventType,
                IsActive = isActive,
            }
        );
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
    private const string ControllerUrl = "events/common/index";
}