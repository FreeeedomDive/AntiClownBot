using System.Net.Sockets;
using System.Text;
using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using Loggers;

namespace AntiClownDiscordBotVersion2.ServicesHealth;

public class ServicesHealthChecker : IServicesHealthChecker
{
    public ServicesHealthChecker(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IAppSettingsService appSettingsService,
        IGuildSettingsService guildSettingsService,
        ILogger logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.appSettingsService = appSettingsService;
        this.guildSettingsService = guildSettingsService;
        this.logger = logger;

        ServiceDescription = new Dictionary<ServiceType, string>()
        {
            { ServiceType.Api, "Api" },
            { ServiceType.AdminApi, "AdminApi" },
            { ServiceType.MinecraftServer, "Minecraft Server" },
        };

        ServiceStatus = new Dictionary<ServiceType, bool>()
        {
            { ServiceType.Api, false },
            { ServiceType.AdminApi, false },
            { ServiceType.MinecraftServer, false },
        };

        ServiceStatusCheck = new Dictionary<ServiceType, Func<Task<bool>>>()
        {
            { ServiceType.Api, IsApiOnline },
            { ServiceType.AdminApi, IsAdminApiOnline },
            { ServiceType.MinecraftServer, IsMinecraftServerOnline },
        };
    }

    public void Start()
    {
        Task.Run(StartScheduler);
    }

    private async Task StartScheduler()
    {
        while (true)
        {
            var statusChanged = false;
            var servicesStatusBuilder = new StringBuilder("Состояние сервисов:");
            var appSettings = appSettingsService.GetSettings();
            var guildSettings = guildSettingsService.GetGuildSettings();

            var checkInterval = appSettings.ServicesCheckIntervalInSeconds * 1000;
            await Task.Delay(checkInterval);

            // collect data about services
            foreach (var serviceType in Enum.GetValues<ServiceType>())
            {
                var currentStatus = await ServiceStatusCheck[serviceType]();
                if (ServiceStatus[serviceType] != currentStatus)
                {
                    statusChanged = true;
                }

                servicesStatusBuilder.Append($"\n{ServiceDescription[serviceType]}: {ConvertBoolToStatus(currentStatus)}");
            }

            var totalStatus = servicesStatusBuilder.ToString();
            logger.Info(totalStatus);

            if (!statusChanged)
            {
                continue;
            }

            // send this data to bot channel
            await discordClientWrapper.Channels.ModifyChannelAsync(guildSettings.BotChannelId,
                model => { model.Topic = totalStatus; });
        }
    }

    private async Task<bool> IsApiOnline()
    {
        return await apiClient.Utility.PingApiAsync();
    }

    private Task<bool> IsAdminApiOnline()
    {
        return Task.FromResult(false);
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

    private Dictionary<ServiceType, string> ServiceDescription { get; }
    private Dictionary<ServiceType, bool> ServiceStatus { get; }
    private Dictionary<ServiceType, Func<Task<bool>>> ServiceStatusCheck { get; }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IAppSettingsService appSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly ILogger logger;
}