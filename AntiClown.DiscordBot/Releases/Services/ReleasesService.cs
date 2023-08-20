using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Releases;
using AntiClown.DiscordBot.Releases.Repositories;

namespace AntiClown.DiscordBot.Releases.Services;

public class ReleasesService : IReleasesService
{
    public ReleasesService(
        ICurrentReleaseProvider currentReleaseProvider,
        IDiscordClientWrapper discordClientWrapper,
        IReleaseEmbedBuilder releaseEmbedBuilder,
        IReleasesRepository releasesRepository,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.releasesRepository = releasesRepository;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.currentReleaseProvider = currentReleaseProvider;
        this.discordClientWrapper = discordClientWrapper;
        this.releaseEmbedBuilder = releaseEmbedBuilder;
    }

    public async Task NotifyIfNewVersionAvailableAsync()
    {
        var currentVersion = currentReleaseProvider.GetCurrentRelease();
        var lastNotifiedVersion = await releasesRepository.ReadLastAsync();
        if (Equals(currentVersion, lastNotifiedVersion))
        {
            return;
        }

        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        await releasesRepository.CreateAsync(currentVersion);
        await discordClientWrapper.Messages.SendAsync(botChannelId, releaseEmbedBuilder.Build(currentVersion));
    }

    private readonly ICurrentReleaseProvider currentReleaseProvider;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IReleaseEmbedBuilder releaseEmbedBuilder;
    private readonly IReleasesRepository releasesRepository;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}