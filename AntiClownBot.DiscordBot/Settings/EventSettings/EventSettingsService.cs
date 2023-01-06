using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Settings.EventSettings;

public class EventSettingsService : IEventSettingsService
{
    public EventSettings GetEventSettings()
    {
        var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ?? throw new Exception("AntiClownBotFilesDirectory env variable was null");
        var fileName = $"{filesDirectory}/SettingsFiles/eventSettings.json";
        var settings = JsonConvert.DeserializeObject<EventSettings>(File.ReadAllText(fileName));
        if (settings == null)
        {
            throw new Exception("Failed to read event settings");
        }

        return settings;
    }
}