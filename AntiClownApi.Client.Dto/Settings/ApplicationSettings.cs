// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace AntiClownApiClient.Dto.Settings;

public class ApplicationSettings
{
    public string DiscordToken { get; set; }
    public string ApiUrl { get; set; }
    public bool MaintenanceMode { get; set; }
    public int ApiPollingIntervalInSeconds { get; set; }
    public bool IsBackendFeedReadingEnabled { get; set; }
    public bool PingOnEvents { get; set; }
}