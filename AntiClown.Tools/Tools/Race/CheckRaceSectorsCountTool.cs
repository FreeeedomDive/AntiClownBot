using AntiClown.Entertainment.Api.Client;
using AntiClown.Tools.Args;

namespace AntiClown.Tools.Tools.Race;

public class CheckRaceSectorsCountTool : ToolBase
{
    public CheckRaceSectorsCountTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IArgsProvider argsProvider,
        ILogger<CheckRaceSectorsCountTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.argsProvider = argsProvider;
    }

    protected override async Task RunAsync()
    {
        var raceId = argsProvider.GetArgument<Guid>("-raceId");
        var race = await antiClownEntertainmentApiClient.CommonEvents.Race.ReadAsync(raceId);
        Logger.LogInformation("Sectors in race: {x}", race.Sectors.Length);
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IArgsProvider argsProvider;
}