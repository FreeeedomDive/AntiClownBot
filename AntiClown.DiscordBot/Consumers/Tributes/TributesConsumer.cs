using AntiClown.Messages.Dto.Tributes;
using MassTransit;
using Newtonsoft.Json;

namespace AntiClown.DiscordBot.Consumers.Tributes;

public class TributesConsumer : IConsumer<TributeMessageDto>
{
    public TributesConsumer(
        ILogger<TributesConsumer> logger
    )
    {
        this.logger = logger;
    }

    public Task Consume(ConsumeContext<TributeMessageDto> context)
    {
        var tribute = context.Message;
        logger.LogInformation(
            "{ConsumerName} received tribute by {UserId}\n{Tribute}",
            ConsumerName,
            tribute.UserId,
            JsonConvert.SerializeObject(tribute)
        );

        return Task.CompletedTask;
    }

    private static string ConsumerName => nameof(TributesConsumer);
    private readonly ILogger<TributesConsumer> logger;
}