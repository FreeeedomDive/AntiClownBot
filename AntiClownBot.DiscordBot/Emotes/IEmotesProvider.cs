using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Emotes;

public interface IEmotesProvider
{
    Task InitializeAsync();
    Task<DiscordEmoji> GetEmoteAsync(string emoteName);
    Task<string> GetEmoteAsTextAsync(string emoteName);
}