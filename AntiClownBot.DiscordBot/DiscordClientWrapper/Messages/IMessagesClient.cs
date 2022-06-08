using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Messages;

public interface IMessagesClient
{
    Task<DiscordMessage> RespondAsync(DiscordMessage message, string content);
    Task<DiscordMessage> RespondAsync(DiscordMessage message, DiscordEmbed embed);
    Task<DiscordMessage> RespondAsync(DiscordMessage message, DiscordMessageBuilder builder);
    Task<DiscordMessage> RespondAsync(InteractionContext context, string? content, InteractionResponseType interactionType = InteractionResponseType.ChannelMessageWithSource);
    Task<DiscordMessage> RespondAsync(InteractionContext context, DiscordEmbed embed);
    Task<DiscordMessage> ModifyAsync(DiscordMessage message, string content);
    Task<DiscordMessage> ModifyAsync(DiscordMessage message, DiscordEmbed embed);
    Task<DiscordMessage> ModifyAsync(DiscordMessage message, DiscordMessageBuilder builder);
    Task<DiscordMessage> ModifyAsync(InteractionContext context, string? content);
    Task<DiscordMessage> ModifyEmbedAsync(InteractionContext context, DiscordEmbed embed, IEnumerable<DiscordComponent>? components = null);
    Task<DiscordMessage> SendAsync(ulong channelId, string content);
    Task<DiscordMessage> SendAsync(ulong channelId, DiscordEmbed embed);
    Task<DiscordMessage> SendAsync(ulong channelId, DiscordMessageBuilder builder);
    Task<DiscordMessage> FindMessageAsync(ulong channelId, ulong messageId);
}