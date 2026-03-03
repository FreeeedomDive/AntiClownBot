using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class CreateCommonEventsSettingsTool : ToolBase
{
    public CreateCommonEventsSettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CreateCommonEventsSettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "GuessNumberEvent.WaitingTimeInMilliseconds", 10 * 60 * 1000);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "LotteryEvent.WaitingTimeInMilliseconds", 10 * 60 * 1000);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "RaceEvent.WaitingTimeInMilliseconds", 10 * 60 * 1000);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "Transfusion.MinimumExchange", 50);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "Transfusion.MaximumExchange", 200);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "Race.Laps", 5);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "Race.GridPositionPenalty", 150);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "CommonEventsWorker.StartHour", 1);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "CommonEventsWorker.StartMinute", 30);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.CommonEvents, "CommonEventsWorker.IterationTime", new TimeSpan(0, 2, 0, 0, 500));
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}