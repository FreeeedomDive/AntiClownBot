using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;

namespace AntiClown.Tools.Tools.Race;

public class CreateDriversTool : ToolBase
{
    public CreateDriversTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<CreateDriversTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var names = new[]
        {
            "Verstappen",
            "Perez",
            "Leclerc",
            "Sainz",
            "Hamilton",
            "Russell",
            "Ocon",
            "Gasly",
            "Piastri",
            "Norris",
            "Bottas",
            "Zhou",
            "Stroll",
            "Alonso",
            "Magnussen",
            "Hulkenberg",
            "DeVries",
            "Tsunoda",
            "Albon",
            "Sargeant"
        };

        var drivers = names.Select(x => new RaceDriverDto
        {
            DriverName = x,
            Points = 0,
            AccelerationSkill = 0.8,
            BreakingSkill = 0.8,
            CorneringSkill = 0.8,
        });

        await Task.WhenAll(drivers.Select(x => antiClownEntertainmentApiClient.CommonEvents.Race.Drivers.CreateAsync(x)));
    }

    public override string Name => nameof(CreateDriversTool);
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}