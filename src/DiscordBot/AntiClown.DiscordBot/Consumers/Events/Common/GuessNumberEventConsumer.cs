using AntiClown.DiscordBot.Interactivity.Services.GuessNumber;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public class GuessNumberEventConsumer : ICommonEventConsumer<GuessNumberEventDto>
{
    public GuessNumberEventConsumer(
        IGuessNumberEventService guessNumberEventService,
        ILogger<GuessNumberEventConsumer> logger
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

            logger.LogInformation(
                "{ConsumerName} received FINISHED event with id {eventId}",
                ConsumerName,
                eventId
            );
            return;
        }

        await guessNumberEventService.CreateAsync(eventId);

        logger.LogInformation(
            "{ConsumerName} received event with id {eventId}",
            ConsumerName,
            eventId
        );
    }

    private string ConsumerName => nameof(GuessNumberEventConsumer);

    private readonly IGuessNumberEventService guessNumberEventService;
    private readonly ILogger<GuessNumberEventConsumer> logger;
}