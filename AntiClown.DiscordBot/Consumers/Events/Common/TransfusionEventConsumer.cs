using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class TransfusionEventConsumer : ICommonEventConsumer<TransfusionEventDto>
{
    public TransfusionEventConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IEmotesCache emotesCache,
        IUsersCache usersCache,
        IOptions<DiscordOptions> discordOptions,
        ILogger<TransfusionEventConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.emotesCache = emotesCache;
        this.usersCache = usersCache;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        var transfusionEvent = await antiClownEntertainmentApiClient.CommonEvents.Transfusion.ReadAsync(eventId);
        var donorMember = await usersCache.GetMemberByApiIdAsync(transfusionEvent.DonorUserId);
        var recipientMember = await usersCache.GetMemberByApiIdAsync(transfusionEvent.RecipientUserId);
        
        var messageContent = "Я решил немного перемешать экономику, " +
                             "поэтому возьму немного скам-койнов у " +
                             $"{donorMember.ServerOrUserName()}. " +
                             $"Отдай {recipientMember.ServerOrUserName()} {transfusionEvent.Exchange} скам-койнов {await emotesCache.GetEmoteAsTextAsync("Okayge")}";
        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, messageContent);
        
        logger.LogInformation("{ConsumerName} received event with id {eventId}", ConsumerName, eventId);
    }

    private static string ConsumerName => nameof(TransfusionEventConsumer);

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IEmotesCache emotesCache;
    private readonly IUsersCache usersCache;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly ILogger<TransfusionEventConsumer> logger;
}