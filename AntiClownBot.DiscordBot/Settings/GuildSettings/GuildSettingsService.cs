using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Settings.GuildSettings;

public class GuildSettingsService : IGuildSettingsService
{
    public AntiClownApiClient.Dto.Settings.GuildSettings GetGuildSettings()
    {
        var settings = JsonConvert.DeserializeObject<AntiClownApiClient.Dto.Settings.GuildSettings>(File.ReadAllText(FileName));
        if (settings == null)
        {
            throw new Exception("Failed to read guild");
        }

        return settings;
    }

    private const string FileName = "../Files/SettingsFiles/guild.json";
}