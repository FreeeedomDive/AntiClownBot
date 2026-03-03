using System.Collections.Concurrent;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Cache.Emotes;

public class EmotesCache : IEmotesCache
{
    public EmotesCache(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<EmotesCache> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.logger = logger;
        emotesCache = new ConcurrentDictionary<string, DiscordEmoji>();
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (isInitialized)
            {
                return;
            }

            var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
            var emotes = await discordClientWrapper.Emotes.GetAllGuildEmojisAsync(guildId);
            emotesCache = new ConcurrentDictionary<string, DiscordEmoji>(emotes.ToDictionary(x => x.Name));
            isInitialized = true;
            logger.LogInformation("Cache has been initialized with {count} emotes", emotesCache.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "{UsersCache} initialization error", nameof(EmotesCache));
        }
    }

    public async Task<DiscordEmoji> GetEmoteAsync(string emoteName)
    {
        if (!isInitialized)
        {
            await InitializeAsync();
        }
        if (emotesCache.TryGetValue(emoteName, out var cachedEmote))
        {
            logger.LogInformation("Get emote {name} from cache", emoteName);
            return cachedEmote;
        }
        var emote = await discordClientWrapper.Emotes.FindEmoteAsync(emoteName);
        emotesCache.TryAdd(emoteName, emote);
        logger.LogInformation("Add emote {name} to cache", emoteName);
        return emote;
    }

    public async Task<string> GetEmoteAsTextAsync(string emoteName)
    {
        var emote = await GetEmoteAsync(emoteName);
        return emote.ToString()!;
    }

    private bool isInitialized;
    private ConcurrentDictionary<string, DiscordEmoji> emotesCache;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ILogger<EmotesCache> logger;
}