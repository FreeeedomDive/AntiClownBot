using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.Entertainment.Api.Dto.DailyEvents.ResetsAndPayments;
using AntiClown.Messages.Dto.Events.Daily;
using DSharpPlus.Entities;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Daily;

public class ResetsAndPaymentsConsumer : IDailyEventConsumer<ResetsAndPaymentsEventDto>
{
    public ResetsAndPaymentsConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<ResetsAndPaymentsConsumer> logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<DailyEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        logger.LogInformation(
            "{ConsumerName} received resets and payments event with id {eventId}",
            ConsumerName,
            eventId
        );
        var embed = new DiscordEmbedBuilder()
                    .WithTitle("Ежедневные выплаты!")
                    .WithColor(DiscordColor.Lilac)
                    .AddField("Все получают по 250 скам-койнов", "А также сброшены лохотроны и магазины!")
                    .Build();

        var botChannelId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "BotChannelId");
        await discordClientWrapper.Messages.SendAsync(botChannelId, embed);
    }

    private static string ConsumerName => nameof(ResetsAndPaymentsConsumer);

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ILogger<ResetsAndPaymentsConsumer> logger;
}