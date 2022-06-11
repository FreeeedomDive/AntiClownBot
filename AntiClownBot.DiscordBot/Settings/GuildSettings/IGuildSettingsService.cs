namespace AntiClownDiscordBotVersion2.Settings.GuildSettings;

public interface IGuildSettingsService
{
    AntiClownApiClient.Dto.Settings.GuildSettings GetGuildSettings();
}