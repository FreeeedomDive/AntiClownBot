using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.RemoveCooldowns;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;

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
        return (await commonEventsRepository.ReadAsync(eventId) as RemoveCoolDownsEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var newEvent = new RemoveCoolDownsEvent
        {
            Id = Guid.NewGuid(),
            Finished = false,
            EventDateTime = DateTime.UtcNow,
        };
        await commonEventsRepository.CreateAsync(newEvent);
        await eventsMessageProducer.ProduceAsync(newEvent);
        await antiClownApiClient.Economy.ResetAllCoolDownsAsync();

        return newEvent.Id;
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IEventsMessageProducer eventsMessageProducer;
}