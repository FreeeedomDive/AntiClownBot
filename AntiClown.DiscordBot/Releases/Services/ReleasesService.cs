using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Releases;
using AntiClown.DiscordBot.Options;
using AntiClown.DiscordBot.Releases.Repositories;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Releases.Services;

public class ReleasesService : IReleasesService
{
    public ReleasesService(
        ICurrentReleaseProvider currentReleaseProvider,
        IDiscordClientWrapper discordClientWrapper,
        IOptions<DiscordOptions> discordOptions,
        IReleaseEmbedBuilder releaseEmbedBuilder,
        IReleasesRepository releasesRepository
    )
    {
        this.releasesRepository = releasesRepository;
        this.currentReleaseProvider = currentReleaseProvider;
        this.discordClientWrapper = discordClientWrapper;
        this.discordOptions = discordOptions;
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

        await releasesRepository.CreateAsync(currentVersion);
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, releaseEmbedBuilder.Build(currentVersion));
    }

    private readonly ICurrentReleaseProvider currentReleaseProvider;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IReleaseEmbedBuilder releaseEmbedBuilder;
    private readonly IReleasesRepository releasesRepository;
}