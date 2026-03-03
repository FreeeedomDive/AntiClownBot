using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class CreateEconomySettingsTool : ToolBase
{
    public CreateEconomySettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CreateEconomySettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Economy, "DefaultTributeCooldown", 60 * 60 * 1000);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Economy, "DefaultScamCoins", 1500);
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}