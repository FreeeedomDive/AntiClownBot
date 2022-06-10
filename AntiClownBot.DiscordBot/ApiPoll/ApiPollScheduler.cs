using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.ApiPoll;

public class ApiPollScheduler : IApiPollScheduler
{
    public ApiPollScheduler(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IAppSettingsService appSettingsService,
        IGuildSettingsService guildSettingsService,
        TributeService tributeService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.appSettingsService = appSettingsService;
        this.guildSettingsService = guildSettingsService;
        this.tributeService = tributeService;
    }

    public void Start()
    {
        Task.Run(StartScheduler);
    }

    private async Task StartScheduler()
    {
        while (true)
        {
            var appSettings = appSettingsService.GetSettings();

            var pollPeriod = appSettings.ApiPollingIntervalInSeconds * 1000;
            await Task.Delay(pollPeriod);

            if (!appSettings.IsBackendFeedReadingEnabled)
            {
                continue;
            }

            var guildSettings = guildSettingsService.GetGuildSettings();
            var isServerWorking = await apiClient.Utility.PingApiAsync();
            if (!isServerWorking)
            {
                var admin = await discordClientWrapper.Members.GetAsync(guildSettings.AdminId);
                var messageBuilder = new DiscordMessageBuilder
                {
                    Content = $"{admin.Mention} сервер прилёг"
                };
                messageBuilder.WithAllowedMention(UserMention.All);
                await discordClientWrapper.Messages.SendAsync(guildSettings.BotChannelId, messageBuilder);
                continue;
            }

            var stalledAutoTributes = await apiClient.Utility.GetAutomaticTributesAsync();
            if (stalledAutoTributes == null || stalledAutoTributes.Count == 0)
            {
                continue;
            }

            foreach (var tribute in stalledAutoTributes)
            {
                var embed = await tributeService.TryMakeEmbedForTribute(tribute);
                if (embed != null)
                {
                    await discordClientWrapper.Messages.SendAsync(guildSettings.TributeChannelId, embed);
                }
            }
        }
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IAppSettingsService appSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly TributeService tributeService;
}