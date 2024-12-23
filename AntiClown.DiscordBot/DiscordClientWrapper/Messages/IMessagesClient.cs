﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Messages;

public interface IMessagesClient
{
    Task<DiscordMessage> RespondAsync(DiscordMessage message, string content);
    Task<DiscordMessage> RespondAsync(DiscordMessage message, DiscordEmbed embed);
    Task<DiscordMessage> RespondAsync(DiscordMessage message, DiscordMessageBuilder builder);

    Task<DiscordMessage> RespondAsync(
        InteractionContext context,
        string? content,
        InteractionResponseType interactionType = InteractionResponseType.ChannelMessageWithSource,
        bool isEphemeral = false
    );

    Task RespondAsync(DiscordInteraction interaction, InteractionResponseType interactionResponseType, DiscordInteractionResponseBuilder? builder);
    Task EditOriginalResponseAsync(DiscordInteraction interaction, DiscordWebhookBuilder builder);
    Task<DiscordMessage> ModifyAsync(DiscordMessage message, string content);
    Task<DiscordMessage> ModifyAsync(DiscordMessage message, DiscordEmbed embed);
    Task<DiscordMessage> ModifyAsync(DiscordMessage message, DiscordMessageBuilder builder);
    Task<DiscordMessage> SendAsync(ulong channelId, string content, bool isThread = false);
    Task<DiscordMessage> SendAsync(ulong channelId, DiscordEmbed embed, bool isThread = false);
    Task<DiscordMessage> SendAsync(ulong channelId, DiscordMessageBuilder builder, bool isThread = false);
    Task<DiscordMessage> FindMessageAsync(ulong channelId, ulong messageId);
    Task<DiscordThreadChannel> CreateThreadAsync(DiscordMessage message, string content);
    Task DeleteAsync(DiscordMessage message);
}