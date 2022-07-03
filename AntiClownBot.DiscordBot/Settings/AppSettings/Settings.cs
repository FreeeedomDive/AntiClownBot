// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace AntiClownDiscordBotVersion2.Settings.AppSettings;

public class Settings
{
    public string DiscordToken { get; set; }
    public string ApiUrl { get; set; }
    public bool MaintenanceMode { get; set; }
    public int ApiPollingIntervalInSeconds { get; set; }
    public int ServicesCheckIntervalInSeconds { get; set; }
    public bool IsBackendFeedReadingEnabled { get; set; }
    public bool PingOnEvents { get; set; }
    public string LogLevel { get; set; }
}