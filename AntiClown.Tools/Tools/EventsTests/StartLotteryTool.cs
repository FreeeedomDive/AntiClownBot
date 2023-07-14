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

        var eventId = await antiClownEntertainmentApiClient.CommonEvents.Lottery.StartNewAsync();
        await antiClownEntertainmentApiClient.CommonEvents.Lottery.AddParticipantAsync(eventId, user.Id);
        var lotteryEvent = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
        Logger.LogInformation(JsonConvert.SerializeObject(lotteryEvent, Formatting.Indented));
        await antiClownEntertainmentApiClient.CommonEvents.Lottery.FinishAsync(eventId);

        userEconomy = await antiClownApiClient.Economy.ReadAsync(user.Id);
        Logger.LogInformation(JsonConvert.SerializeObject(userEconomy, Formatting.Indented));
    }

    public override string Name => nameof(StartLotteryTool);
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}