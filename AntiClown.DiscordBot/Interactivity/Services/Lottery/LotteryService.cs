using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Lottery;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.Interactivity.Services.Lottery;

public class LotteryService : ILotteryService
{
    public LotteryService(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IInteractivityRepository interactivityRepository,
        ILotteryEmbedBuilder lotteryEmbedBuilder,
        IUsersCache usersCache,
        IEmotesCache emotesCache,
        IOptions<DiscordOptions> discordOptions,
        IOptions<Settings> settings
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.interactivityRepository = interactivityRepository;
        this.lotteryEmbedBuilder = lotteryEmbedBuilder;
        this.usersCache = usersCache;
        this.discordOptions = discordOptions;
        this.emotesCache = emotesCache;
        this.settings = settings;
    }

    public async Task StartAsync(Guid eventId)
    {
        var lotteryEvent = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(lotteryEvent);
        var message = await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, messageBuilder);
        var interactivity = new Interactivity<object>
        {
            Id = eventId,
            Type = InteractivityType.Lottery,
            AuthorId = 0,
            MessageId = message.Id,
        };
        await interactivityRepository.CreateAsync(interactivity);
    }

    public async Task AddParticipantAsync(Guid eventId, ulong memberId)
    {
        var userId = await usersCache.GetApiIdByMemberIdAsync(memberId);
        await antiClownEntertainmentApiClient.CommonEvents.Lottery.AddParticipantAsync(eventId, userId);
        var lotteryEvent = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(lotteryEvent);
        var interactivity = await interactivityRepository.TryReadAsync<object>(eventId);
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(discordOptions.Value.BotChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);
    }

    public async Task FinishAsync(Guid eventId)
    {
        var interactivity = await interactivityRepository.TryReadAsync<object>(eventId);
        var lotteryEvent = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(lotteryEvent);
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(discordOptions.Value.BotChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);
    }

    private async Task<DiscordMessageBuilder> BuildEventMessageAsync(LotteryEventDto lotteryEventDto)
    {
        var ping = settings.Value.PingOnEvents ? "@everyone" : "";
        return new DiscordMessageBuilder()
               .WithAllowedMentions(Mentions.All)
               .WithContent(ping)
               .WithEmbed(await lotteryEmbedBuilder.BuildAsync(lotteryEventDto))
               .AddComponents(
                   new DiscordButtonComponent(
                       !lotteryEventDto.Finished
                           ? ButtonStyle.Primary
                           : ButtonStyle.Secondary,
                       InteractionsIds.EventsButtons.Lottery.BuildId(lotteryEventDto.Id, InteractionsIds.EventsButtons.Lottery.Join),
                       lotteryEventDto.Participants.Count.ToPluralizedString("участник", "участника", "участников"),
                       lotteryEventDto.Finished,
                       new DiscordComponentEmoji(await emotesCache.GetEmoteAsync("NOTED"))
                   )
               );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly IEmotesCache emotesCache;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly ILotteryEmbedBuilder lotteryEmbedBuilder;
    private readonly IOptions<Settings> settings;
    private readonly IUsersCache usersCache;
}