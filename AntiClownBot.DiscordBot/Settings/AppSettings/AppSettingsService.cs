using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Settings.AppSettings;

public class AppSettingsService : IAppSettingsService
{
    public Settings GetSettings()
    {
        var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FileName));
        if (settings == null)
        {
            throw new Exception("Failed to read settings");
        }

        return settings;
    }

    private const string FileName = "../Files/SettingsFiles/settings.json";
}