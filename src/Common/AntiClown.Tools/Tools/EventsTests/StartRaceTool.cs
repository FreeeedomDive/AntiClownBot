using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Tools.Tools.EventsTests;

public class StartRaceTool : ToolBase
{
    public StartRaceTool(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<StartRaceTool> logger
    ) : base(logger)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var raceId = await antiClownEntertainmentApiClient.RaceEvent.StartNewAsync();
        Logger.LogInformation("RaceId {id}", raceId);
        var users = await antiClownApiClient.Users.ReadAllAsync();
        await antiClownEntertainmentApiClient.RaceEvent.AddParticipantAsync(raceId, users.SelectRandomItem().Id);
        await antiClownEntertainmentApiClient.RaceEvent.FinishAsync(raceId);
        try
        {
            await antiClownEntertainmentApiClient.RaceEvent.FinishAsync(raceId);
        }
        catch (EventAlreadyFinishedException e)
        {
            Logger.LogInformation("Event was already finished\n{Exception}", e);
        }

        var race = await antiClownEntertainmentApiClient.RaceEvent.ReadAsync(raceId);
        var lastSector = race.Sectors.Last().DriversOnSector.OrderBy(x => x.TotalTime).ToArray();
        Logger.LogInformation("Last sector info:\n{info}", string.Join("\n", lastSector.Select(x => $"{x.DriverName}   {x.TotalTime}")));
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}