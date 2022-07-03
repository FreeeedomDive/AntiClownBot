using System.Net.Sockets;
using System.Text;
using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;

namespace AntiClownDiscordBotVersion2.ServicesHealth;

public class ServicesHealthChecker : IServicesHealthChecker
{
    public ServicesHealthChecker(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IAppSettingsService appSettingsService,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.appSettingsService = appSettingsService;
        this.guildSettingsService = guildSettingsService;
    }

    public void Start()
    {
        Task.Run(StartScheduler);
    }

    private async Task StartScheduler()
    {
        while (true)
        {
            var servicesStatusBuilder = new StringBuilder("Состояние сервисов:");
            var appSettings = appSettingsService.GetSettings();
            var guildSettings = guildSettingsService.GetGuildSettings();

            var checkInterval = appSettings.ServicesCheckIntervalInSeconds * 1000;
            await Task.Delay(checkInterval);

            // collect data about services
            servicesStatusBuilder.Append($"\nAPI: {ConvertBoolToStatus(await IsApiOnline())}.");
            servicesStatusBuilder.Append($"\nMinecraft server: {ConvertBoolToStatus(await IsMinecraftServerOnline())}.");

            // send this data to bot channel
            await discordClientWrapper.Channels.ModifyChannelAsync(guildSettings.BotChannelId, model =>
            {
                model.Topic = servicesStatusBuilder.ToString();
            });
        }
    }

    private async Task<bool> IsApiOnline()
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        return await apiClient.Utility.PingApiAsync();
    }

    private async Task<bool> IsMinecraftServerOnline()
    {
        using var tcpClient = new TcpClient();
        var guildSettings = guildSettingsService.GetGuildSettings();
        tcpClient.SendTimeout = 1000;
        tcpClient.ReceiveTimeout = 1000;
        try
        {
            await tcpClient.ConnectAsync(guildSettings.MinecraftServerLocalAddress, guildSettings.MinecraftServerPort);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ConvertBoolToStatus(bool online)
    {
        return online ? "ONLINE" : "OFFLINE";
    }
    
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IAppSettingsService appSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
}