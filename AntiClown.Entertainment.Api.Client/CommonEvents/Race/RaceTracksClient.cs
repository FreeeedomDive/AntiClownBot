﻿using AntiClown.Entertainment.Api.Client.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using RestSharp;

namespace AntiClown.Entertainment.Api.Client.CommonEvents.Race;

public class RaceTracksClient : IRaceTracksClient
{
    public RaceTracksClient(RestClient restClient)
    {
        this.restClient = restClient;
    }

    public async Task<RaceTrackDto[]> ReadAllAsync()
    {
        var request = new RestRequest(ControllerUrl);
        var response = await restClient.ExecuteGetAsync(request);
        return response.TryDeserialize<RaceTrackDto[]>();
    }

    public async Task CreateAsync(RaceTrackDto raceTrack)
    {
        var request = new RestRequest(ControllerUrl);
        request.AddJsonBody(raceTrack);
        var response = await restClient.ExecutePostAsync(request);
        response.ThrowIfNotSuccessful();
    }

    private readonly RestClient restClient;

    private const string ControllerUrl = "events/common/race/tracks";
}