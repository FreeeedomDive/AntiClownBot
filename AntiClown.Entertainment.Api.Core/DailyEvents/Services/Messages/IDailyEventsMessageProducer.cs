using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services.Messages;

public interface IDailyEventsMessageProducer
{
    Task ProduceAsync(DailyEventBase eventBase);
}