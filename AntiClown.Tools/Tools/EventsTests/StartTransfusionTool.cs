﻿using AntiClown.Entertainment.Api.Client;
using Newtonsoft.Json;

namespace AntiClown.Tools.Tools.EventsTests;

public class StartTransfusionTool : ToolBase
{
    public StartTransfusionTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<StartTransfusionTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var eventId = await antiClownEntertainmentApiClient.TransfusionEvent.StartNewAsync();
        var transfusionEvent = await antiClownEntertainmentApiClient.TransfusionEvent.ReadAsync(eventId);
        Logger.LogInformation(JsonConvert.SerializeObject(transfusionEvent, Formatting.Indented));
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}