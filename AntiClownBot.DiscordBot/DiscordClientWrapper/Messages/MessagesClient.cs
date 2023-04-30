using AntiClownDiscordBotVersion2.DiscordClientWrapper.Emotes;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Messages;

public class MessagesClient : IMessagesClient
{
    public MessagesClient(
        DiscordClient discordClient,
        IEmotesClient emotesClient,
        IGuildSettingsService guildSettingsService,
        ILoggerClient logger
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
        InteractionResponseType interactionType = InteractionResponseType.ChannelMessageWithSource,
        bool isEphemeral = false
    )
    {
        var builder = content == null ? null : new DiscordInteractionResponseBuilder().WithContent(content).AsEphemeral(isEphemeral);
        await context.CreateResponseAsync(interactionType, builder);
        return await context.GetOriginalResponseAsync();
    }

    public async Task<DiscordMessage> RespondAsync(InteractionContext context, DiscordEmbed? embed, bool isEphemeral = false)
    {
        var builder = embed == null ? null : new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral(isEphemeral);
        await context.CreateResponseAsync(builder);
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
        return await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(content ?? $" {await emotesClient.FindEmoteAsync("YEP")} "));
    }

    public async Task<DiscordMessage> ModifyAsync(InteractionContext context, DiscordWebhookBuilder builder)
    {
        return await context.EditResponseAsync(builder);
    }

    public async Task RespondAsync(DiscordInteraction interaction, InteractionResponseType interactionResponseType, DiscordInteractionResponseBuilder? builder)
    {
        await interaction.CreateResponseAsync(interactionResponseType, builder);
    }

    public async Task EditOriginalResponseAsync(DiscordInteraction interaction, DiscordWebhookBuilder builder)
    {
        await interaction.EditOriginalResponseAsync(builder);
    }

    public async Task<DiscordMessage> SendAsync(ulong channelId, string content)
    {
        var guild = guildSettingsService.GetGuildSettings().GuildId;
        var message = await discordClient.Guilds[guild]
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

    public async Task DeleteAsync(InteractionContext context)
    {
        await context.DeleteResponseAsync();
    }

    public async Task DeleteAsync(DiscordMessage message)
    {
        await message.DeleteAsync();
    }

    public async Task<DiscordThreadChannel> CreateThreadAsync(DiscordMessage message, string content)
    {
        if (content.Length >= ThreadNameLengthLimit)
        {
            content = $"{content[..30]}...";
        }
        return await message.CreateThreadAsync(content, AutoArchiveDuration.Day);
    }

    private readonly DiscordClient discordClient;
    private readonly IEmotesClient emotesClient;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly ILoggerClient logger;

    private const int ThreadNameLengthLimit = 100;
}