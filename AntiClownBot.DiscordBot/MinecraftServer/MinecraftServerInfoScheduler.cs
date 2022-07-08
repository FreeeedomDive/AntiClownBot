using System.Text;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using CommonServices.IpService;
using CommonServices.MinecraftServerService;
using Loggers;

namespace AntiClownDiscordBotVersion2.MinecraftServer;

public class MinecraftServerInfoScheduler : IMinecraftServerInfoScheduler
{
    public MinecraftServerInfoScheduler(
        IDiscordClientWrapper discordClientWrapper,
        IAppSettingsService appSettingsService,
        IGuildSettingsService guildSettingsService,
        IMinecraftServerInfoService minecraftServerInfoService,
        IIpService ipService,
        ILogger logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.appSettingsService = appSettingsService;
        this.guildSettingsService = guildSettingsService;
        this.minecraftServerInfoService = minecraftServerInfoService;
        this.ipService = ipService;
        this.logger = logger;
    }

    public void Start()
    {
        Task.Run(StartScheduler);
    }

    private async Task StartScheduler()
    {
        while (true)
        {
            await Wait();

            var ip = await ipService.GetIp();
            if (ip == null)
            {
                logger.Info("Ip was null");
                continue;
            }

            var guildSettings = guildSettingsService.GetGuildSettings();
            
            var serverInfo = await minecraftServerInfoService.ReadServerInfo($"{ip}{guildSettings.MinecraftServerPort}");
            if (serverInfo == null)
            {
                logger.Info("Server info was null");
                continue;
            }

            var messageBuilder = new StringBuilder();
            var isServerOnline = serverInfo.Online;
            messageBuilder
                .Append($"Сервер онлайн: {ConvertBoolToStatus(isServerOnline)}\n")
                .Append($"Версия: {serverInfo.Version}\n")
                .Append($"Ip: {serverInfo.Ip}:{serverInfo.Port}");
            if (isServerOnline)
            {
                messageBuilder.Append($"\nИгроков онлайн: {serverInfo.Players.Online} / {serverInfo.Players.Max}");
                if (serverInfo.Players.List != null)
                {
                    messageBuilder
                        .Append('\n')
                        .Append(string.Join('\n', serverInfo.Players.List));
                }
            }
            
            await discordClientWrapper.Channels.ModifyChannelAsync(guildSettings.MinecraftChannelId, model =>
            {
                model.Topic = messageBuilder.ToString();
            });
        }
    }

    private async Task Wait()
    {
        var appSettings = appSettingsService.GetSettings();
        var checkInterval = appSettings.MinecraftServerCheckIntervalInSeconds * 1000;
        await Task.Delay(checkInterval);
    }

    private static string ConvertBoolToStatus(bool online)
    {
        return online ? "✅" : "❌";
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IAppSettingsService appSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IMinecraftServerInfoService minecraftServerInfoService;
    private readonly IIpService ipService;
    private readonly ILogger logger;
}