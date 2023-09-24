using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Tools.Tools.SettingsCreators;

public class CreateDiscordGuildSettingsTool : ToolBase
{
    public CreateDiscordGuildSettingsTool(
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CreateDiscordGuildSettingsTool> logger
    ) : base(logger)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    protected override async Task RunAsync()
    {
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "GuildId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "AdminId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "BotChannelId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "TributeChannelId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "PartyChannelId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "HiddenTestChannelId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "DotaRoleId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "CsRoleId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "SiGameRoleId", 0);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "JoinRolePrice", 1000);
        await antiClownDataApiClient.Settings.CreateOrUpdateAsync(SettingsCategory.DiscordGuild, "CreateRolePrice", 2500);
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}