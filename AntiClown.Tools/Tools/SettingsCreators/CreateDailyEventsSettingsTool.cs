using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class CreateDailyEventsSettingsTool : ToolBase
{
    public CreateDailyEventsSettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CreateDailyEventsSettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DailyEvents, "DailyEventsWorker.StartHour", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DailyEvents, "DailyEventsWorker.StartMinute", 1);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DailyEvents, "DailyEventsWorker.IterationTime", new TimeSpan(1, 0, 0, 6, 0));
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}