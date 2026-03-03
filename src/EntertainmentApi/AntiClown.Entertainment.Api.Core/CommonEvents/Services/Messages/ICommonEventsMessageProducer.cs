using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;

public interface ICommonEventsMessageProducer
{
    Task ProduceAsync(CommonEventBase eventBase);
}