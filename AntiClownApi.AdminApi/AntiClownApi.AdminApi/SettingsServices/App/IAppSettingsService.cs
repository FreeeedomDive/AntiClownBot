using AntiClownApiClient.Dto.Settings;

namespace AntiClownApi.AdminApi.SettingsServices.App;

public interface IAppSettingsService
{
    ApplicationSettings GetSettings();
    void UpdateSettings(ApplicationSettings newSettings);
}