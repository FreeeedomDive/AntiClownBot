namespace AntiClown.Data.Api.Core.SettingsStoring.Repositories;

public interface ISettingsRepository
{
    Task<SettingStorageElement[]> ReadAllAsync();
    Task<SettingStorageElement?> TryReadAsync(string category, string key);
    Task<SettingStorageElement[]> FindAsync(string category);
    Task CreateOrUpdateAsync(string category, string key, string value);
}