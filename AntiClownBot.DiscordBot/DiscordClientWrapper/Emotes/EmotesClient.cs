﻿using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Emotes;

public class EmotesClient : IEmotesClient
{
    public EmotesClient(
        DiscordClient discordClient,
        IGuildSettingsService guildSettingsService,
        ILogger logger
    )
    {
        this.discordClient = discordClient;
        this.guildSettingsService = guildSettingsService;
        this.logger = logger;
    }

    public Task<DiscordEmoji> FindEmoteAsync(string emoteName)
    {
        var correctEmoteName =
            (emoteName.StartsWith(":") ? "" : ":")
            + emoteName
            + (emoteName.EndsWith(":") ? "" : ":");
        var hasEmote = DiscordEmoji.TryFromName(discordClient, correctEmoteName, true, out var emote);
        if (!hasEmote)
        {
            throw new ArgumentException($"Emote {emoteName} doesn't exist");
        }

        return Task.FromResult(emote);
    }

    public async Task AddReactionToMessageAsync(DiscordMessage message, string emoteName)
    {
        var emote = await FindEmoteAsync(emoteName);
        await AddReactionToMessageAsync(message, emote);
    }

    public async Task AddReactionToMessageAsync(DiscordMessage message, DiscordEmoji emote)
    {
        await message.CreateReactionAsync(emote);
    }

    public async Task RemoveReactionFromMessageAsync(DiscordMessage message, DiscordEmoji emote, DiscordUser user)
    {
        await message.DeleteReactionAsync(emote, user);
    }

    private readonly DiscordClient discordClient;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly ILogger logger;
}