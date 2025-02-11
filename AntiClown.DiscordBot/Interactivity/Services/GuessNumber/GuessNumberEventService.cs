﻿using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.GuessNumber;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.Interactivity.Services.GuessNumber;

public class GuessNumberEventService : IGuessNumberEventService
{
    public GuessNumberEventService(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IInteractivityRepository interactivityRepository,
        IGuessNumberEmbedBuilder guessNumberEmbedBuilder,
        IAntiClownDataApiClient antiClownDataApiClient,
        IUsersCache usersCache,
        IEmotesCache emotesCache
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.interactivityRepository = interactivityRepository;
        this.guessNumberEmbedBuilder = guessNumberEmbedBuilder;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public async Task CreateAsync(Guid eventId)
    {
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var guessNumberEvent = await antiClownEntertainmentApiClient.GuessNumberEvent.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(guessNumberEvent);
        var message = await discordClientWrapper.Messages.SendAsync(botChannelId, messageBuilder);
        var interactivity = new Interactivity<object>
        {
            Id = eventId,
            Type = InteractivityType.GuessNumber,
            AuthorId = 0,
            MessageId = message.Id,
        };
        await interactivityRepository.CreateAsync(interactivity);
    }

    public async Task AddUserPickAsync(Guid eventId, ulong memberId, GuessNumberPickDto pick)
    {
        var userId = await usersCache.GetApiIdByMemberIdAsync(memberId);
        await antiClownEntertainmentApiClient.GuessNumberEvent.AddPickAsync(eventId, new GuessNumberUserPickDto
        {
            UserId = userId, 
            Pick = pick,
        });
        var guessNumberEvent = await antiClownEntertainmentApiClient.GuessNumberEvent.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(guessNumberEvent);
        var interactivity = await interactivityRepository.TryReadAsync<object>(eventId);
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);
    }

    public async Task FinishAsync(Guid eventId)
    {
        var interactivity = await interactivityRepository.TryReadAsync<object>(eventId);
        var guessNumberEvent = await antiClownEntertainmentApiClient.GuessNumberEvent.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(guessNumberEvent);
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);
    }

    private async Task<DiscordMessageBuilder> BuildEventMessageAsync(GuessNumberEventDto guessNumberEvent)
    {
        var ping = await antiClownDataApiClient.Settings.ReadBoolAsync(SettingsCategory.DiscordBot, "PingOnEvents") ? "@everyone" : "";
        return new DiscordMessageBuilder()
               .WithContent(ping)
               .WithEmbed(await guessNumberEmbedBuilder.BuildAsync(guessNumberEvent))
               .AddComponents(
                   new DiscordButtonComponent(
                       !guessNumberEvent.Finished
                           ? ButtonStyle.Primary
                           : guessNumberEvent.Result == GuessNumberPickDto.One
                               ? ButtonStyle.Success
                               : ButtonStyle.Secondary,
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(guessNumberEvent.Id, 1),
                       "(" + (guessNumberEvent.NumberToUsers.TryGetValue(GuessNumberPickDto.One, out var users1) ? users1.Count : 0) + ")",
                       guessNumberEvent.Finished,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("one"))
                   ),
                   new DiscordButtonComponent(
                       !guessNumberEvent.Finished
                           ? ButtonStyle.Primary
                           : guessNumberEvent.Result == GuessNumberPickDto.Two
                               ? ButtonStyle.Success
                               : ButtonStyle.Secondary,
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(guessNumberEvent.Id, 2),
                       "(" + (guessNumberEvent.NumberToUsers.TryGetValue(GuessNumberPickDto.Two, out var users2) ? users2.Count : 0) + ")",
                       guessNumberEvent.Finished,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("two"))
                   ),
                   new DiscordButtonComponent(
                       !guessNumberEvent.Finished
                           ? ButtonStyle.Primary
                           : guessNumberEvent.Result == GuessNumberPickDto.Three
                               ? ButtonStyle.Success
                               : ButtonStyle.Secondary,
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(guessNumberEvent.Id, 3),
                       "(" + (guessNumberEvent.NumberToUsers.TryGetValue(GuessNumberPickDto.Three, out var users3) ? users3.Count : 0) + ")",
                       guessNumberEvent.Finished,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("three"))
                   ),
                   new DiscordButtonComponent(
                       !guessNumberEvent.Finished
                           ? ButtonStyle.Primary
                           : guessNumberEvent.Result == GuessNumberPickDto.Four
                               ? ButtonStyle.Success
                               : ButtonStyle.Secondary,
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(guessNumberEvent.Id, 4),
                       "(" + (guessNumberEvent.NumberToUsers.TryGetValue(GuessNumberPickDto.Four, out var users4) ? users4.Count : 0) + ")",
                       guessNumberEvent.Finished,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("four"))
                   )
               );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesCache emotesCache;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IGuessNumberEmbedBuilder guessNumberEmbedBuilder;
    private readonly IUsersCache usersCache;
}