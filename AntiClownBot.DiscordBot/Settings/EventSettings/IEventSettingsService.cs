namespace AntiClownDiscordBotVersion2.Settings.EventSettings;

public interface IEventSettingsService
{
    AntiClownApiClient.Dto.Settings.EventSettings GetEventSettings();
}