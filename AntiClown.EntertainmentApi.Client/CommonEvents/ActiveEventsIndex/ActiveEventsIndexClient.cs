﻿using AntiClown.EntertainmentApi.Client.Extensions;
using AntiClown.EntertainmentApi.Dto.CommonEvents;
using AntiClown.EntertainmentApi.Dto.CommonEvents.ActiveEventsIndex;
using RestSharp;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.ActiveEventsIndex;

public class ActiveEventsIndexClient : IActiveEventsIndexClient
{
    public ActiveEventsIndexClient(RestClient restClient)
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
        request.AddJsonBody(new ActiveEventIndexDto
        {
            EventType = eventType,
            IsActive = isActiveByDefault
        });
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    public async Task UpdateAsync(CommonEventTypeDto eventType, bool isActive)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(new ActiveEventIndexDto
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
    private const string ControllerUrl = "events/common/index";
}