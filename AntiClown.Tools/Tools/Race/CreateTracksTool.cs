using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;

namespace AntiClown.Tools.Tools.Race;

public class CreateTracksTool : ToolBase
{
    public CreateTracksTool(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<CreateTracksTool> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var tracks = new[]
        {
            new RaceTrackDto
            {
                Name = "Bahrain",
                IdealTime = 91447,
                CorneringDifficulty = 9,
                AccelerationDifficulty = 26,
                BreakingDifficulty = 15,
            },
            new RaceTrackDto
            {
                Name = "Saudi Arabia",
                IdealTime = 90734,
                CorneringDifficulty = 23,
                AccelerationDifficulty = 17,
                BreakingDifficulty = 10,
            },
            new RaceTrackDto
            {
                Name = "Australia",
                IdealTime = 80260,
                CorneringDifficulty = 12,
                AccelerationDifficulty = 19,
                BreakingDifficulty = 19,
            },
            new RaceTrackDto
            {
                Name = "Azerbaijan",
                IdealTime = 103009,
                CorneringDifficulty = 14,
                AccelerationDifficulty = 17,
                BreakingDifficulty = 19,
            },
            new RaceTrackDto
            {
                Name = "Miami",
                IdealTime = 91361,
                CorneringDifficulty = 11,
                AccelerationDifficulty = 19,
                BreakingDifficulty = 20,
            },
            new RaceTrackDto
            {
                Name = "Emilia Romagna",
                IdealTime = 75484,
                CorneringDifficulty = 21,
                AccelerationDifficulty = 12,
                BreakingDifficulty = 17,
            },
            new RaceTrackDto
            {
                Name = "Monaco",
                IdealTime = 72909,
                CorneringDifficulty = 30,
                AccelerationDifficulty = 8,
                BreakingDifficulty = 12,
            },
            new RaceTrackDto
            {
                Name = "Spain",
                IdealTime = 78149,
                CorneringDifficulty = 23,
                AccelerationDifficulty = 17,
                BreakingDifficulty = 10,
            },
            new RaceTrackDto
            {
                Name = "Canada",
                IdealTime = 73078,
                CorneringDifficulty = 11,
                AccelerationDifficulty = 24,
                BreakingDifficulty = 15,
            },
            new RaceTrackDto
            {
                Name = "Austria",
                IdealTime = 65619,
                CorneringDifficulty = 10,
                AccelerationDifficulty = 24,
                BreakingDifficulty = 16,
            },
            new RaceTrackDto
            {
                Name = "Great Britain",
                IdealTime = 87097,
                CorneringDifficulty = 25,
                AccelerationDifficulty = 10,
                BreakingDifficulty = 15,
            },
            new RaceTrackDto
            {
                Name = "Hungary",
                IdealTime = 76627,
                CorneringDifficulty = 22,
                AccelerationDifficulty = 12,
                BreakingDifficulty = 16,
            },
            new RaceTrackDto
            {
                Name = "Belgium",
                IdealTime = 106286,
                CorneringDifficulty = 14,
                AccelerationDifficulty = 25,
                BreakingDifficulty = 11,
            },
            new RaceTrackDto
            {
                Name = "Netherlands",
                IdealTime = 71097,
                CorneringDifficulty = 23,
                AccelerationDifficulty = 12,
                BreakingDifficulty = 15,
            },
            new RaceTrackDto
            {
                Name = "Italy",
                IdealTime = 81046,
                CorneringDifficulty = 9,
                AccelerationDifficulty = 29,
                BreakingDifficulty = 12,
            },
            new RaceTrackDto
            {
                Name = "Singapore",
                IdealTime = 101905,
                CorneringDifficulty = 25,
                AccelerationDifficulty = 11,
                BreakingDifficulty = 14,
            },
            new RaceTrackDto
            {
                Name = "Japan",
                IdealTime = 90983,
                CorneringDifficulty = 21,
                AccelerationDifficulty = 12,
                BreakingDifficulty = 17,
            },
            new RaceTrackDto
            {
                Name = "Qatar",
                IdealTime = 83196,
                CorneringDifficulty = 15,
                AccelerationDifficulty = 19,
                BreakingDifficulty = 16,
            },
            new RaceTrackDto
            {
                Name = "USA",
                IdealTime = 96169,
                CorneringDifficulty = 21,
                AccelerationDifficulty = 14,
                BreakingDifficulty = 15,
            },
            new RaceTrackDto
            {
                Name = "Mexico",
                IdealTime = 77774,
                CorneringDifficulty = 17,
                AccelerationDifficulty = 20,
                BreakingDifficulty = 13,
            },
            new RaceTrackDto
            {
                Name = "Brazil",
                IdealTime = 70540,
                CorneringDifficulty = 19,
                AccelerationDifficulty = 15,
                BreakingDifficulty = 16,
            },
            new RaceTrackDto
            {
                Name = "Las Vegas",
                IdealTime = 82000,
                CorneringDifficulty = 16,
                AccelerationDifficulty = 20,
                BreakingDifficulty = 14,
            },
            new RaceTrackDto
            {
                Name = "Abu Dhabi",
                IdealTime = 86103,
                CorneringDifficulty = 18,
                AccelerationDifficulty = 18,
                BreakingDifficulty = 14,
            },
        };

        await Task.WhenAll(tracks.Select(x => antiClownEntertainmentApiClient.CommonEvents.Race.Tracks.CreateAsync(x)));
    }

    public override string Name => nameof(CreateTracksTool);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}