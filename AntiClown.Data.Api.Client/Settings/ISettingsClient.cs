/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Data.Api.Client.Settings;

public interface ISettingsClient
{
    Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> ReadAllAsync();
    Task<AntiClown.Data.Api.Dto.Settings.SettingDto> ReadAsync(string category, string key);
    Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> FindAsync(string category);
    Task CreateOrUpdateAsync(AntiClown.Data.Api.Dto.Settings.SettingDto settingDto);
}
