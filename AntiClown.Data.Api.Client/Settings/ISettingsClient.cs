/* Generated file */
namespace AntiClown.Data.Api.Client.Settings;

public interface ISettingsClient
{
    System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> ReadAllAsync();
    System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Settings.SettingDto> ReadAsync(System.String category, System.String key);
    System.Threading.Tasks.Task<AntiClown.Data.Api.Dto.Settings.SettingDto[]> FindAsync(System.String category);
    System.Threading.Tasks.Task CreateOrUpdateAsync(AntiClown.Data.Api.Dto.Settings.SettingDto settingDto);
}
