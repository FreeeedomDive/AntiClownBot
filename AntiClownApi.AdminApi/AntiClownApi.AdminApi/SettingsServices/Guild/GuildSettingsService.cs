using AntiClownApiClient.Dto.Settings;
using Newtonsoft.Json;

namespace AntiClownApi.AdminApi.SettingsServices.Guild;

public class GuildSettingsService : IGuildSettingsService
{
    public GuildSettings GetGuildSettings()
    {
        var settings = JsonConvert.DeserializeObject<GuildSettings>(File.ReadAllText(FileName));
        if (settings == null)
        {
            throw new Exception("Failed to read guild");
        }

        return settings;
    }

    public void UpdateSettings(GuildSettings newSettings)
    {
        File.WriteAllText(FileName, JsonConvert.SerializeObject(newSettings));
    }

    private const string FileName = "../Files/SettingsFiles/guild.json";
}