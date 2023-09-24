using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Lottery;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Interactivity.Domain;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using DSharpPlus;
using DSharpPlus.Entities;

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
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.interactivityRepository = interactivityRepository;
        this.lotteryEmbedBuilder = lotteryEmbedBuilder;
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public async Task StartAsync(Guid eventId)
    {
        var lotteryEvent = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(lotteryEvent);
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var message = await discordClientWrapper.Messages.SendAsync(botChannelId, messageBuilder);
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
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);
    }

    public async Task FinishAsync(Guid eventId)
    {
        var interactivity = await interactivityRepository.TryReadAsync<object>(eventId);
        var lotteryEvent = await antiClownEntertainmentApiClient.CommonEvents.Lottery.ReadAsync(eventId);
        var messageBuilder = await BuildEventMessageAsync(lotteryEvent);
        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        var oldMessage = await discordClientWrapper.Messages.FindMessageAsync(botChannelId, interactivity!.MessageId);
        await discordClientWrapper.Messages.ModifyAsync(oldMessage, messageBuilder);
    }

    private async Task<DiscordMessageBuilder> BuildEventMessageAsync(LotteryEventDto lotteryEventDto)
    {
        var ping = await antiClownDataApiClient.Settings.ReadBoolAsync(SettingsCategory.DiscordBot, "PingOnEvents") ? "@everyone" : "";
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
    private readonly IEmotesCache emotesCache;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IInteractivityRepository interactivityRepository;
    private readonly ILotteryEmbedBuilder lotteryEmbedBuilder;
    private readonly IUsersCache usersCache;
}