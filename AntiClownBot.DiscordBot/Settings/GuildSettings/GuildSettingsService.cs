using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Settings.GuildSettings;

public class GuildSettingsService : IGuildSettingsService
{
    public GuildSettings GetGuildSettings()
    {
        var filesDirectory = Environment.GetEnvironmentVariable("AntiClownBotFilesDirectory") ?? throw new Exception("AntiClownBotFilesDirectory env variable was null");
        var fileName = $"{filesDirectory}/SettingsFiles/guild.json";
        var settings = JsonConvert.DeserializeObject<GuildSettings>(File.ReadAllText(fileName));
        if (settings == null)
        {
            throw new Exception("Failed to read guild");
        }

        return settings;
    }
}