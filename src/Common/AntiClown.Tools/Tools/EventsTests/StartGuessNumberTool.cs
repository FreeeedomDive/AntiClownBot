using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
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

        var eventId = await antiClownEntertainmentApiClient.GuessNumberEvent.StartNewAsync();
        var guessNumberEvent = await antiClownEntertainmentApiClient.GuessNumberEvent.ReadAsync(eventId);
        await antiClownEntertainmentApiClient.GuessNumberEvent.AddPickAsync(eventId, new GuessNumberUserPickDto
        {
            UserId = user.Id,
            Pick = guessNumberEvent.Result,
        });
        await antiClownEntertainmentApiClient.GuessNumberEvent.FinishAsync(eventId);

        userEconomy = await antiClownApiClient.Economy.ReadAsync(user.Id);
        Logger.LogInformation(JsonConvert.SerializeObject(userEconomy, Formatting.Indented));
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}