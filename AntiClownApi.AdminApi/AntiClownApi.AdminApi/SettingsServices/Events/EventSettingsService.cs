using AntiClownApiClient.Dto.Settings;
using Newtonsoft.Json;

namespace AntiClownApi.AdminApi.SettingsServices.Events;

public class EventSettingsService : IEventSettingsService
{
    public EventSettings GetEventSettings()
    {
        var settings = JsonConvert.DeserializeObject<EventSettings>(File.ReadAllText(FileName));
        if (settings == null)
        {
            throw new Exception("Failed to read event settings");
        }

        return settings;
    }

    public void UpdateSettings(EventSettings newSettings)
    {
        File.WriteAllText(FileName, JsonConvert.SerializeObject(newSettings));
    }

    private const string FileName = "../Files/SettingsFiles/eventSettings.json";
}