using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Settings.AppSettings;

public class AppSettingsService : IAppSettingsService
{
    public Settings GetSettings()
    {
        var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ?? throw new Exception("AntiClownBotFilesDirectory env variable was null");
        var fileName = $"{filesDirectory}/SettingsFiles/settings.json";
        var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(fileName));
        if (settings == null)
        {
            throw new Exception("Failed to read settings");
        }

        return settings;
    }
}