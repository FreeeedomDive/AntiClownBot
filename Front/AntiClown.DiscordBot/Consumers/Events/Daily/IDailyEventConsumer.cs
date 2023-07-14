using AntiClown.EntertainmentApi.Dto.DailyEvents;
using AntiClown.Messages.Dto.Events.Daily;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.Events.Daily;

public interface IDailyEventConsumer<T> where T : DailyEventBaseDto
{
    Task ConsumeAsync(ConsumeContext<DailyEventMessageDto> context);
}