using AntiClown.Data.Api.Client.Settings;
using AntiClown.Data.Api.Dto.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using Newtonsoft.Json;

namespace AntiClown.Data.Api.Client.Extensions;

public static class SettingsClientExtensions
{
    public static async Task<SettingDto> ReadAsync(this ISettingsClient settingsClient, SettingsCategory category, string key)
    {
        return await settingsClient.ReadAsync(category.ToString(), key);
    }

    public static async Task<T> ReadAsync<T>(this ISettingsClient settingsClient, string category, string key) where T : IParsable<T>
    {
        var setting = await settingsClient.ReadAsync(category, key);
        return setting.GetValue<T>();
    }

    public static async Task<T> ReadAsync<T>(this ISettingsClient settingsClient, SettingsCategory category, string key) where T : IParsable<T>
    {
        return await settingsClient.ReadAsync<T>(category.ToString(), key);
    }

    public static async Task<T> ReadAndDeserializeJsonAsync<T>(this ISettingsClient settingsClient, string category, string key)
    {
        var setting = await settingsClient.ReadAsync(category, key);
        return setting.DeserializeJsonValue<T>();
    }

    public static async Task<T> ReadAndDeserializeJsonAsync<T>(this ISettingsClient settingsClient, SettingsCategory category, string key)
    {
        return await settingsClient.ReadAndDeserializeJsonAsync<T>(category.ToString(), key);
    }

    public static async Task CreateOrUpdateAsync<T>(this ISettingsClient settingsClient, string category, string key, T value) where T : notnull
    {
        await settingsClient.CreateOrUpdateAsync(category, key, value.ToString()!);
    }

    public static async Task CreateOrUpdateAsync<T>(this ISettingsClient settingsClient, SettingsCategory category, string key, T value) where T : notnull
    {
        await settingsClient.CreateOrUpdateAsync(category.ToString(), key, value);
    }

    public static async Task CreateOrUpdateSerializedAsync<T>(this ISettingsClient settingsClient, string category, string key, T value) where T : notnull
    {
        await settingsClient.CreateOrUpdateAsync(
            category, key, JsonConvert.SerializeObject(
                value, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                }
            )
        );
    }

    public static async Task CreateOrUpdateSerializedAsync<T>(this ISettingsClient settingsClient, SettingsCategory category, string key, T value) where T : notnull
    {
        await settingsClient.CreateOrUpdateSerializedAsync(category.ToString(), key, value);
    }
}