using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Cache.Emotes;

public interface IEmotesCache
{
    Task InitializeAsync();
    Task<DiscordEmoji> GetEmoteAsync(string emoteName);
    Task<string> GetEmoteAsTextAsync(string emoteName);
}