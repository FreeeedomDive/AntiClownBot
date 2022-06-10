using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Emotes;

public interface IEmotesClient
{
    Task<DiscordEmoji> FindEmoteAsync(string emoteName);
    Task AddReactionToMessageAsync(DiscordMessage message, string emoteName);
    Task AddReactionToMessageAsync(DiscordMessage message, DiscordEmoji emote);
    Task RemoveReactionFromMessageAsync(DiscordMessage message, DiscordEmoji emote, DiscordUser user);
}