using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.RemoveCoolDowns;

public class RemoveCoolDownsEventService : IRemoveCoolDownsEventService
{
    public RemoveCoolDownsEventService(
        IAntiClownApiClient antiClownApiClient,
        ICommonEventsRepository commonEventsRepository,
        IEventsMessageProducer eventsMessageProducer
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.commonEventsRepository = commonEventsRepository;
        this.eventsMessageProducer = eventsMessageProducer;
    }

    public async Task<RemoveCoolDownsEvent> ReadAsync(Guid eventId)
    {
        var @event = await commonEventsRepository.ReadAsync(eventId);
        if (@event.Type != CommonEventType.RemoveCoolDowns)
        {
            throw new WrongEventTypeException($"Event {eventId} is not a remove cooldown");
        }

        return (@event as RemoveCoolDownsEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var newEvent = RemoveCoolDownsEvent.Create();
        await commonEventsRepository.CreateAsync(newEvent);
        await eventsMessageProducer.ProduceAsync(newEvent);
        await antiClownApiClient.Economy.ResetAllCoolDownsAsync();

        return newEvent.Id;
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IEventsMessageProducer eventsMessageProducer;
}