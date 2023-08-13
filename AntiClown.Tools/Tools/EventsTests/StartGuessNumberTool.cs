using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Tools.Utility.Extensions;
using Newtonsoft.Json;

namespace AntiClown.Tools.Tools.EventsTests;

public class StartGuessNumberTool : ToolBase
{
    public StartGuessNumberTool(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<StartGuessNumberTool> logger
    ) : base(logger)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override async Task RunAsync()
    {
        var user = (await antiClownApiClient.Users.ReadAllAsync()).SelectRandomItem();
        var userEconomy = await antiClownApiClient.Economy.ReadAsync(user.Id);
        Logger.LogInformation(JsonConvert.SerializeObject(userEconomy, Formatting.Indented));

        var eventId = await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.StartNewAsync();
        var guessNumberEvent = await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.ReadAsync(eventId);
        await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.AddPickAsync(eventId, user.Id, guessNumberEvent.Result);
        await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.FinishAsync(eventId);

        userEconomy = await antiClownApiClient.Economy.ReadAsync(user.Id);
        Logger.LogInformation(JsonConvert.SerializeObject(userEconomy, Formatting.Indented));
    }

    public override string Name => nameof(StartGuessNumberTool);
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}