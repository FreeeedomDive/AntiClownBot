using AntiClown.Data.Api.Dto.Settings;
using Newtonsoft.Json;

namespace AntiClown.Data.Api.Dto.Extensions;

public static class SettingDtoExtensions
{
    public static T GetValue<T>(this SettingDto settingDto) where T : IParsable<T>
    {
        return T.Parse(settingDto.Value, null);
    }

    public static T DeserializeJsonValue<T>(this SettingDto settingDto)
    {
        return JsonConvert.DeserializeObject<T>(
            settingDto.Value, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }
        )!;
    }
}