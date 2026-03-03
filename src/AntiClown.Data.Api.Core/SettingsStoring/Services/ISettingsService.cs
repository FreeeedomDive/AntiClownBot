using AntiClown.Data.Api.Core.SettingsStoring.Domain;

namespace AntiClown.Data.Api.Core.SettingsStoring.Services;

public interface ISettingsService
{
    Task<Setting[]> ReadAllAsync();
    Task<Setting> ReadAsync(string category, string key);
    Task<Setting[]> FindAsync(string category);
    Task CreateOrUpdateAsync(string category, string key, string value);
}