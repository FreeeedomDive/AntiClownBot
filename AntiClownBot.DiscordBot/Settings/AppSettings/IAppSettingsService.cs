namespace AntiClownDiscordBotVersion2.Settings.AppSettings;

public interface IAppSettingsService
{
    AntiClownApiClient.Dto.Settings.ApplicationSettings GetSettings();
}