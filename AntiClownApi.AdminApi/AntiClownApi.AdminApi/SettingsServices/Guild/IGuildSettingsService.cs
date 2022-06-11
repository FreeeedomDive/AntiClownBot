using AntiClownApiClient.Dto.Settings;

namespace AntiClownApi.AdminApi.SettingsServices.Guild;

public interface IGuildSettingsService
{
    GuildSettings GetGuildSettings();
    void UpdateSettings(GuildSettings newSettings);
}