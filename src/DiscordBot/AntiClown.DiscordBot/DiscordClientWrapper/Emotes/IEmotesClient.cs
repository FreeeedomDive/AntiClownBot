using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Emotes;

public interface IEmotesClient
{
    Task<DiscordEmoji[]> GetAllGuildEmojisAsync(ulong guildId);
    Task<DiscordEmoji> FindEmoteAsync(string emoteName);
    Task AddReactionToMessageAsync(DiscordMessage message, string emoteName);
    Task AddReactionToMessageAsync(DiscordMessage message, DiscordEmoji emote);
    Task RemoveReactionFromMessageAsync(DiscordMessage message, DiscordEmoji emote, DiscordUser user);
}