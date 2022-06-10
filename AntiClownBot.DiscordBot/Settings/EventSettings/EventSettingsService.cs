using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Settings.EventSettings;

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

    private const string FileName = "../Files/SettingsFiles/eventSettings.json";
}