using AntiClown.Api.Client;
using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
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
        var raceId = await antiClownEntertainmentApiClient.CommonEvents.Race.StartNewAsync();
        Logger.LogInformation("RaceId {id}", raceId);
        var users = await antiClownApiClient.Users.ReadAllAsync();
        await antiClownEntertainmentApiClient.CommonEvents.Race.AddParticipantAsync(raceId, users.SelectRandomItem().Id);
        await antiClownEntertainmentApiClient.CommonEvents.Race.FinishAsync(raceId);
        try
        {
            await antiClownEntertainmentApiClient.CommonEvents.Race.FinishAsync(raceId);
        }
        catch (EventAlreadyFinishedException e)
        {
            Logger.LogInformation("Event was already finished\n{Exception}", e);
        }

        var race = await antiClownEntertainmentApiClient.CommonEvents.Race.ReadAsync(raceId);
        var lastSector = race.Sectors.Last().DriversOnSector.OrderBy(x => x.TotalTime).ToArray();
        Logger.LogInformation("Last sector info:\n{info}", string.Join("\n", lastSector.Select(x => $"{x.DriverName}   {x.TotalTime}")));
    }

    public override string Name => nameof(StartRaceTool);

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}