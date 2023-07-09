using System.Collections.Concurrent;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Emotes;

public class EmotesProvider : IEmotesProvider
{
    public EmotesProvider(
        IDiscordClientWrapper discordClientWrapper,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.guildSettingsService = guildSettingsService;
        emotesCache = new ConcurrentDictionary<string, DiscordEmoji>();
    }

    public async Task InitializeAsync()
    {
        var guildId = guildSettingsService.GetGuildSettings().GuildId;
        var emotes = await discordClientWrapper.Emotes.GetAllGuildEmojisAsync(guildId);
        emotesCache = new ConcurrentDictionary<string, DiscordEmoji>(emotes.ToDictionary(x => x.Name));
    }

    public async Task<DiscordEmoji> GetEmoteAsync(string emoteName)
    {
        if (emotesCache.TryGetValue(emoteName, out var cachedEmote))
        {
            return cachedEmote;
        }
        var emote = await discordClientWrapper.Emotes.FindEmoteAsync(emoteName);
        emotesCache.TryAdd(emoteName, emote);
        return emote;
    }

    public async Task<string> GetEmoteAsTextAsync(string emoteName)
    {
        var emote = await GetEmoteAsync(emoteName);
        return emote.ToString()!;
    }

    private ConcurrentDictionary<string, DiscordEmoji> emotesCache;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IGuildSettingsService guildSettingsService;
}