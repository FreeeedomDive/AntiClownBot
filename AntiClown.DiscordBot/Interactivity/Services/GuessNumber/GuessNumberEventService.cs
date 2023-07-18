using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Domain.GuessNumber;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Tools.Utility.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Interactivity.Services.GuessNumber;

public class GuessNumberEventService : IGuessNumberEventService
{
    public GuessNumberEventService(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IInteractivityRepository interactivityRepository,
        IUsersCache usersCache,
        IEmotesCache emotesCache,
        IOptions<Settings> settings,
        IOptions<DiscordOptions> discordOptions
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.interactivityRepository = interactivityRepository;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
        this.settings = settings;
        this.discordOptions = discordOptions;
    }

    public async Task CreateAsync(Guid eventId)
    {
        var guessNumberEvent = await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.ReadAsync(eventId);
        var messageBuilder = await BuildWelcomeMessageAsync(eventId, guessNumberEvent);
        var message = await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, messageBuilder);
        var interactivity = new Interactivity<GuessNumberEventDetails>
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
        await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.AddPickAsync(eventId, userId, pick);
        var guessNumberEvent = await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.ReadAsync(eventId);
        var messageBuilder = await BuildWelcomeMessageAsync(eventId, guessNumberEvent);
        var interactivity = await interactivityRepository.TryReadAsync<GuessNumberEventDetails>(eventId);
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(discordOptions.Value.BotChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);
    }

    public async Task FinishAsync(Guid eventId)
    {
        var interactivity = await interactivityRepository.TryReadAsync<GuessNumberEventDetails>(eventId);
        var guessNumberEvent = await antiClownEntertainmentApiClient.CommonEvents.GuessNumber.ReadAsync(eventId);
        var messageBuilder = await BuildWelcomeMessageAsync(eventId, guessNumberEvent);
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(discordOptions.Value.BotChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);

        var winnersMessageContent = await BuildResultsMessageAsync(eventId, guessNumberEvent);
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, winnersMessageContent);
    }

    private async Task<DiscordMessageBuilder> BuildWelcomeMessageAsync(Guid eventId, GuessNumberEventDto guessNumberEvent)
    {
        var ping = DateTime.UtcNow.IsNightTime() || !settings.Value.PingOnEvents ? "" : "@everyone ";
        var embed = new DiscordEmbedBuilder()
                    .WithTitle("Угадай число")
                    .AddField($"{ping}Я загадал число, угадайте его!", "У вас 10 минут")
                    .WithColor(guessNumberEvent.Finished ? DiscordColor.Black : DiscordColor.Azure)
                    .WithFooter($"EventId: {guessNumberEvent.Id}")
                    .Build();
        return new DiscordMessageBuilder()
               .WithEmbed(embed)
               .AddComponents(
                   new DiscordButtonComponent(
                       !guessNumberEvent.Finished
                           ? ButtonStyle.Primary
                           : guessNumberEvent.Result == GuessNumberPickDto.One
                               ? ButtonStyle.Success
                               : ButtonStyle.Secondary,
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(eventId, 1),
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
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(eventId, 2),
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
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(eventId, 3),
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
                       InteractionsIds.EventsButtons.GuessNumber.BuildId(eventId, 4),
                       "(" + (guessNumberEvent.NumberToUsers.TryGetValue(GuessNumberPickDto.Four, out var users4) ? users4.Count : 0) + ")",
                       guessNumberEvent.Finished,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("four"))
                   )
               );
    }

    private async Task<DiscordMessageBuilder> BuildResultsMessageAsync(Guid eventId, GuessNumberEventDto guessNumberEvent)
    {
        var winnersIds = guessNumberEvent.NumberToUsers.TryGetValue(guessNumberEvent.Result, out var result) ? result.ToArray() : Array.Empty<Guid>();
        var winners = await Task.WhenAll(winnersIds.Select(x => usersCache.GetMemberByApiIdAsync(x)));
        var winnersString = winners.Length == 0 
            ? "Никто не угадал и никто не получает добычу-коробку"
            : $"Правильно угадавшие игроки получают добычу-коробку:\n{string.Join("\n", winners.Select(x => x.ServerOrUserName()))}";
        var embed = new DiscordEmbedBuilder()
                    .WithTitle("Угадай число")
                    .AddField($"Правильный ответ: {(int)guessNumberEvent.Result}", winnersString)
                    .WithColor(DiscordColor.White)
                    .WithFooter($"EventId: {guessNumberEvent.Id}")
                    .Build();

        return new DiscordMessageBuilder().AddEmbed(embed);
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEmotesCache emotesCache;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly IOptions<Settings> settings;
    private readonly IUsersCache usersCache;
}