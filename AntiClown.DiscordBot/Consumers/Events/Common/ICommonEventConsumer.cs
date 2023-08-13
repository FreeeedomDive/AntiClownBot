using AntiClown.Entertainment.Api.Dto.CommonEvents;
using AntiClown.Messages.Dto.Events.Common;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Common;

public interface ICommonEventConsumer<T> where T : CommonEventBaseDto
{
    Task ConsumeAsync(ConsumeContext<CommonEventMessageDto> context);
}