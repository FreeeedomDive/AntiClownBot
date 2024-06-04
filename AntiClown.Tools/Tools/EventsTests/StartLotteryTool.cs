using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Tools.Utility.Extensions;
using Newtonsoft.Json;

namespace AntiClown.Tools.Tools.EventsTests;

public class StartLotteryTool : ToolBase
{
    public StartLotteryTool(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<StartLotteryTool> logger
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

        var eventId = await antiClownEntertainmentApiClient.LotteryEvent.StartNewAsync();
        await antiClownEntertainmentApiClient.LotteryEvent.AddParticipantAsync(eventId, user.Id);
        var lotteryEvent = await antiClownEntertainmentApiClient.LotteryEvent.ReadAsync(eventId);
        Logger.LogInformation(JsonConvert.SerializeObject(lotteryEvent, Formatting.Indented));
        await antiClownEntertainmentApiClient.LotteryEvent.FinishAsync(eventId);

        userEconomy = await antiClownApiClient.Economy.ReadAsync(user.Id);
        Logger.LogInformation(JsonConvert.SerializeObject(userEconomy, Formatting.Indented));
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}