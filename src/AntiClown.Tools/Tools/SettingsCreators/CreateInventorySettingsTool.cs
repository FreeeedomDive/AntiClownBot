using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class CreateInventorySettingsTool : ToolBase
{
    public CreateInventorySettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CreateInventorySettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Inventory, "MaximumActiveItemsOfOneType", 3);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Inventory, "SellItemPercent", 50);
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}