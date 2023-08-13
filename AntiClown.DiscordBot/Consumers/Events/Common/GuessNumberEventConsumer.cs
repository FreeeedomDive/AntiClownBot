using AntiClown.DiscordBot.Interactivity.Services.GuessNumber;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class GuessNumberEventConsumer : ICommonEventConsumer<GuessNumberEventDto>
{
    public GuessNumberEventConsumer(
        IGuessNumberEventService guessNumberEventService,
        ILoggerClient logger
    )
    {
        this.guessNumberEventService = guessNumberEventService;
        this.logger = logger;
    }

    public async Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context)
    {
        var eventId = context.Message.EventId;
        if (context.Message.Finished)
        {
            await guessNumberEventService.FinishAsync(eventId);

            await logger.InfoAsync(
                "{ConsumerName} received FINISHED event with id {eventId}",
                ConsumerName,
                eventId
            );
            return;
        }

        await guessNumberEventService.CreateAsync(eventId);

        await logger.InfoAsync(
            "{ConsumerName} received event with id {eventId}",
            ConsumerName,
            eventId
        );
    }

    private string ConsumerName => nameof(GuessNumberEventConsumer);

    private readonly IGuessNumberEventService guessNumberEventService;
    private readonly ILoggerClient logger;
}