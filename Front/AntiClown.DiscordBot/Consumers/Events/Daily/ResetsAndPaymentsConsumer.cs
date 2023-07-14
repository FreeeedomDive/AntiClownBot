using AntiClown.EntertainmentApi.Dto.DailyEvents.ResetsAndPayments;
using AntiClown.Messages.Dto.Events.Daily;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Daily;

public class ResetsAndPaymentsConsumer : IDailyEventConsumer<ResetsAndPaymentsEventDto>
{
    public ResetsAndPaymentsConsumer(ILogger<ResetsAndPaymentsConsumer> logger)
    {
        this.logger = logger;
    }

    public Task ConsumeAsync(ConsumeContext<DailyEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        logger.LogInformation(
            "{ConsumerName} received resets and payments event with id {eventId}",
            ConsumerName,
            eventId
        );

        return Task.CompletedTask;
    }

    private static string ConsumerName => nameof(ResetsAndPaymentsConsumer);

    private readonly ILogger<ResetsAndPaymentsConsumer> logger;
}