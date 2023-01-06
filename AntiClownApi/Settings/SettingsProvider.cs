using System;
using System.IO;
using Newtonsoft.Json;

namespace AntiClownBotApi.Settings;

public class SettingsProvider : ISettingsProvider
{
    public AppSettings GetSettings()
    {
        var data = File.ReadAllText("Settings/settings.json");
        return JsonConvert.DeserializeObject<AppSettings>(data) ?? throw new Exception("Can't deserialize settings");
    }
}