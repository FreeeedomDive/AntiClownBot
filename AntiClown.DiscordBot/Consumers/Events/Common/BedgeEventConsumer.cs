using AntiClown.Entertainment.Api.Dto.CommonEvents.Bedge;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class BedgeEventConsumer : ICommonEventConsumer<BedgeEventDto>
{
    public BedgeEventConsumer(ILogger<BedgeEventConsumer> logger)
    {
        this.logger = logger;
    }

    public Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        logger.LogInformation(
            "{ConsumerName} received bedge event with id {eventId}",
            ConsumerName,
            eventId
        );

        return Task.CompletedTask;
    }

    private static string ConsumerName => nameof(BedgeEventConsumer);

    private readonly ILogger<BedgeEventConsumer> logger;
}