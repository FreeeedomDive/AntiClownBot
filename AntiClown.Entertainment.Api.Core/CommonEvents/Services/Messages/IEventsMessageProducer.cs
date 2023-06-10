using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;

public interface IEventsMessageProducer
{
    Task ProduceAsync(CommonEventBase eventBase);
}