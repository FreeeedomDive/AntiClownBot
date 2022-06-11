using AntiClownApiClient.Dto.Settings;

namespace AntiClownApi.AdminApi.SettingsServices.Events;

public interface IEventSettingsService
{
    EventSettings GetEventSettings();
    void UpdateSettings(EventSettings newSettings);
}