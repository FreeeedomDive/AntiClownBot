using AntiClownApiClient.Dto.Settings;
using Newtonsoft.Json;

namespace AntiClownApi.AdminApi.SettingsServices.App;

public class AppSettingsService : IAppSettingsService
{
    public ApplicationSettings GetSettings()
    {
        var settings = JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText(FileName));
        if (settings == null)
        {
            throw new Exception("Failed to read settings");
        }

        return settings;
    }

    public void UpdateSettings(ApplicationSettings newSettings)
    {
        File.WriteAllText(FileName, JsonConvert.SerializeObject(newSettings));
    }

    private const string FileName = "../Files/SettingsFiles/settings.json";
}