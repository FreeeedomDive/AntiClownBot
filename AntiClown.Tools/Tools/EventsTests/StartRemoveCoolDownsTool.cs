using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Tools.Utility.Extensions;
using Newtonsoft.Json;

namespace AntiClown.Tools.Tools.EventsTests;

public class StartRemoveCoolDownsTool : ToolBase
{
    public StartRemoveCoolDownsTool(
        IAntiClownApiClient antiClownApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<StartRemoveCoolDownsTool> logger
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

        await antiClownEntertainmentApiClient.CommonEvents.RemoveCoolDowns.StartNewAsync();
        userEconomy = await antiClownApiClient.Economy.ReadAsync(user.Id);
        Logger.LogInformation(JsonConvert.SerializeObject(userEconomy, Formatting.Indented));
    }

    public override string Name => nameof(StartRemoveCoolDownsTool);
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}