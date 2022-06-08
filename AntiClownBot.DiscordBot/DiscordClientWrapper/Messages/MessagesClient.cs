using AntiClownDiscordBotVersion2.DiscordClientWrapper.Emotes;
using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Messages;

public class MessagesClient : IMessagesClient
{
    public MessagesClient(
        DiscordClient discordClient,
        IEmotesClient emotesClient,
        IGuildSettingsService guildSettingsService,
        ILogger logger
    )
    {
        this.discordClient = discordClient;
        this.emotesClient = emotesClient;
        this.guildSettingsService = guildSettingsService;
        this.logger = logger;
    }

    public async Task<DiscordMessage> RespondAsync(DiscordMessage message, string content)
    {
        var response = await message.RespondAsync(content);

        return response;
    }

    public async Task<DiscordMessage> RespondAsync(DiscordMessage message, DiscordEmbed embed)
    {
        var response = await message.RespondAsync(embed);

        return response;
    }

    public async Task<DiscordMessage> RespondAsync(DiscordMessage message, DiscordMessageBuilder builder)
    {
        var response = await message.RespondAsync(builder);

        return response;
    }

    public async Task<DiscordMessage> RespondAsync(InteractionContext context,
        string? content = null,
        InteractionResponseType interactionType = InteractionResponseType.ChannelMessageWithSource
    )
    {
        var builder = content == null ? null : new DiscordInteractionResponseBuilder().WithContent(content);
        await context.CreateResponseAsync(interactionType, builder);
        return await context.GetOriginalResponseAsync();
    }

    public async Task<DiscordMessage> RespondAsync(InteractionContext context, DiscordEmbed embed)
    {
        await context.CreateResponseAsync(embed);
        return await context.GetOriginalResponseAsync();
    }

    public async Task<DiscordMessage> ModifyAsync(DiscordMessage message, string content)
    {
        var modified = await message.ModifyAsync(content);

        return modified;
    }

    public async Task<DiscordMessage> ModifyAsync(DiscordMessage message, DiscordEmbed embed)
    {
        var modified = await message.ModifyAsync(embed);

        return modified;
    }

    public async Task<DiscordMessage> ModifyAsync(DiscordMessage message, DiscordMessageBuilder builder)
    {
        var modified = await message.ModifyAsync(builder);

        return modified;
    }

    public async Task<DiscordMessage> ModifyAsync(InteractionContext context, string? content)
    {
        return await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(content ?? $" {await emotesClient.FindEmoteAsync("white_check_mark")} "));
    }

    public async Task<DiscordMessage> ModifyEmbedAsync(InteractionContext context, DiscordWebhookBuilder builder)
    {
        return await context.EditResponseAsync(builder);
    }

    public async Task<DiscordMessage> SendAsync(ulong channelId, string content)
    {
        var message = await discordClient.Guilds[277096298761551872]
            .GetChannel(channelId)
            .SendMessageAsync(content);

        return message;
    }

    public async Task<DiscordMessage> SendAsync(ulong channelId, DiscordEmbed embed)
    {
        var guild = guildSettingsService.GetGuildSettings().GuildId;
        var message = await discordClient.Guilds[guild]
            .GetChannel(channelId)
            .SendMessageAsync(embed);

        return message;
    }

    public async Task<DiscordMessage> SendAsync(ulong channelId, DiscordMessageBuilder builder)
    {
        var guild = guildSettingsService.GetGuildSettings().GuildId;
        var message = await discordClient
            .Guilds[guild]
            .GetChannel(channelId)
            .SendMessageAsync(builder);

        return message;
    }

    public async Task<DiscordMessage> FindMessageAsync(ulong channelId, ulong messageId)
    {
        var guild = guildSettingsService.GetGuildSettings().GuildId;
        var message = await discordClient
            .Guilds[guild]
            .GetChannel(channelId)
            .GetMessageAsync(messageId);

        return message;
    }

    private readonly DiscordClient discordClient;
    private readonly IEmotesClient emotesClient;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly ILogger logger;
}