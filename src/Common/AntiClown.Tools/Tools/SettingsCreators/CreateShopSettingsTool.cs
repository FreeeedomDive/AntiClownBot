using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class CreateShopSettingsTool : ToolBase
{
    public CreateShopSettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CreateShopSettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Shop, "MaximumItemsInShop", 5);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Shop, "DefaultReRollPrice", 100);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Shop, "DefaultReRollPriceIncrease", 25);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Shop, "FreeItemRevealsPerDay", 2);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.Shop, "RevealShopItemPercent", 40);
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}