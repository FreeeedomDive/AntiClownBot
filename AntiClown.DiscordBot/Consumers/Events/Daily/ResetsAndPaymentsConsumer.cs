using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Dto.DailyEvents.ResetsAndPayments;
using AntiClown.Messages.Dto.Events.Daily;
using DSharpPlus.Entities;
using MassTransit;
using Microsoft.Extensions.Options;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Events.Daily;

public class ResetsAndPaymentsConsumer : IDailyEventConsumer<ResetsAndPaymentsEventDto>
{
    public ResetsAndPaymentsConsumer(
        IDiscordClientWrapper discordClientWrapper,
        IOptions<DiscordOptions> discordOptions,
        ILoggerClient logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.discordOptions = discordOptions;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<DailyEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        await logger.InfoAsync(
            "{ConsumerName} received resets and payments event with id {eventId}",
            ConsumerName,
            eventId
        );
        var embed = new DiscordEmbedBuilder()
                    .WithTitle("Ежедневные выплаты!")
                    .WithColor(DiscordColor.Lilac)
                    .AddField("Все получают по 250 скам-койнов", "А также сброшены лохотроны и магазины!")
                    .Build();

        await discordClientWrapper.Messages.SendAsync(discordOptions.Value.BotChannelId, embed);
    }

    private static string ConsumerName => nameof(ResetsAndPaymentsConsumer);

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IOptions<DiscordOptions> discordOptions;
    private readonly ILoggerClient logger;
}