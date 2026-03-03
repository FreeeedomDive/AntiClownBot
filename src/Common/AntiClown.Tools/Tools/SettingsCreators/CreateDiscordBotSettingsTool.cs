using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class CreateDiscordBotSettingsTool : ToolBase
{
    public CreateDiscordBotSettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CreateDiscordBotSettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordBot, "XddAnswersEnabled", true);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordBot, "IsEmoteNotificationEnabled", true);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordBot, "PingOnEvents", false);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordBot, "LogLevel", "Debug");
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}