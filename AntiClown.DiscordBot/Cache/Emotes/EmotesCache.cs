using System.Collections.Concurrent;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Options;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Cache.Emotes;

public class EmotesCache : IEmotesCache
{
    public EmotesCache(
        IDiscordClientWrapper discordClientWrapper,
        IOptions<DiscordOptions> discordOptions,
        ILogger<EmotesCache> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.discordOptions = discordOptions;
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

            var guildId = discordOptions.Value.GuildId;
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
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly ILogger<EmotesCache> logger;
}