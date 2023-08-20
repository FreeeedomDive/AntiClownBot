using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Messages;

public class MessagesClient : IMessagesClient
{
    public MessagesClient(
        DiscordClient discordClient,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.discordClient = discordClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
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

    public async Task<DiscordMessage> RespondAsync(
        InteractionContext context,
        string? content = null,
        InteractionResponseType interactionType = InteractionResponseType.ChannelMessageWithSource,
        bool isEphemeral = false
    )
    {
        var builder = content == null ? null : new DiscordInteractionResponseBuilder().WithContent(content).AsEphemeral(isEphemeral);
        await context.CreateResponseAsync(interactionType, builder);
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

    public async Task RespondAsync(DiscordInteraction interaction, InteractionResponseType interactionResponseType, DiscordInteractionResponseBuilder? builder)
    {
        await interaction.CreateResponseAsync(interactionResponseType, builder);
    }

    public async Task EditOriginalResponseAsync(DiscordInteraction interaction, DiscordWebhookBuilder builder)
    {
        await interaction.EditOriginalResponseAsync(builder);
    }

    public async Task<DiscordMessage> SendAsync(ulong channelId, string content, bool isThread = false)
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var guild = discordClient.Guilds[guildId];
        return isThread switch
        {
            true => await guild.Threads[channelId].SendMessageAsync(content),
            false => await guild.Channels[channelId].SendMessageAsync(content),
        };
    }

    public async Task<DiscordMessage> SendAsync(ulong channelId, DiscordEmbed embed, bool isThread = false)
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var guild = discordClient.Guilds[guildId];
        return isThread switch
        {
            true => await guild.Threads[channelId].SendMessageAsync(embed),
            false => await guild.Channels[channelId].SendMessageAsync(embed),
        };
    }

    public async Task<DiscordMessage> SendAsync(ulong channelId, DiscordMessageBuilder builder, bool isThread = false)
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var guild = discordClient.Guilds[guildId];
        return isThread switch
        {
            true => await guild.Threads[channelId].SendMessageAsync(builder),
            false => await guild.Channels[channelId].SendMessageAsync(builder),
        };
    }

    public async Task<DiscordMessage> FindMessageAsync(ulong channelId, ulong messageId)
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var guild = discordClient.Guilds[guildId];
        var message = await guild.GetChannel(channelId).GetMessageAsync(messageId);

        return message;
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

    public async Task<DiscordMessage> ModifyAsync(InteractionContext context, DiscordWebhookBuilder builder)
    {
        return await context.EditResponseAsync(builder);
    }

    public async Task DeleteAsync(InteractionContext context)
    {
        await context.DeleteResponseAsync();
    }

    private readonly DiscordClient discordClient;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;

    private const int ThreadNameLengthLimit = 100;
}