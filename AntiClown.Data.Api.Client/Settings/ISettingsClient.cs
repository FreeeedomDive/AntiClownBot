using AntiClown.Data.Api.Dto.Settings;

namespace AntiClown.Data.Api.Client.Settings;

public interface ISettingsClient
{
    Task<SettingDto[]> ReadAllAsync();
    Task<SettingDto> ReadAsync(string category, string key);
    Task<SettingDto[]> FindAsync(string category);
    Task CreateOrUpdateAsync(string category, string key, string value);
}