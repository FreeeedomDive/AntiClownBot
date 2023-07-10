using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Core.Common;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Transfusion;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using AntiClown.Tools.Utility.Extensions;
using AntiClown.Tools.Utility.Random;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Transfusion;

public class TransfusionEventService : ITransfusionEventService
{
    public TransfusionEventService(
        ICommonEventsRepository commonEventsRepository,
        IAntiClownApiClient antiClownApiClient,
        IEventsMessageProducer eventsMessageProducer
    )
    {
        this.commonEventsRepository = commonEventsRepository;
        this.antiClownApiClient = antiClownApiClient;
        this.eventsMessageProducer = eventsMessageProducer;
    }

    public async Task<TransfusionEvent> ReadAsync(Guid eventId)
    {
        var @event = await commonEventsRepository.ReadAsync(eventId);
        if (@event.Type != CommonEventType.Transfusion)
        {
            throw new WrongEventTypeException($"Event {eventId} is not a transfusion");
        }

        return (@event as TransfusionEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var newEvent = TransfusionEvent.Create();

        var allUsers = await antiClownApiClient.Users.ReadAllAsync();
        var donor = allUsers.SelectRandomItem();
        var recipient = allUsers.Except(new[] { donor }).SelectRandomItem();
        var exchange = Randomizer.GetRandomNumberInclusive(Constants.TransfusionMinimumExchange, Constants.TransfusionMaximumExchange);

        newEvent.DonorUserId = donor.Id;
        newEvent.RecipientUserId = recipient.Id;
        newEvent.Exchange = exchange;
        await commonEventsRepository.CreateAsync(newEvent);

        await antiClownApiClient.Economy.UpdateScamCoinsAsync(donor.Id, -exchange, $"Событие перекачки {newEvent.Id}");
        await antiClownApiClient.Economy.UpdateScamCoinsAsync(recipient.Id, exchange, $"Событие перекачки {newEvent.Id}");

        await eventsMessageProducer.ProduceAsync(newEvent);

        return newEvent.Id;
    }

    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IEventsMessageProducer eventsMessageProducer;
}