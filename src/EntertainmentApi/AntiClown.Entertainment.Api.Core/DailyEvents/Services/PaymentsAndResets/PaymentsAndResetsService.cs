using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain.PaymentsAndResets;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.Messages;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services.PaymentsAndResets;

public class PaymentsAndResetsService : IPaymentsAndResetsService
{
    public PaymentsAndResetsService(
        IAntiClownApiClient antiClownApiClient,
        IDailyEventsMessageProducer dailyEventsMessageProducer
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.dailyEventsMessageProducer = dailyEventsMessageProducer;
    }

    public Task<PaymentsAndResetsEvent> ReadAsync(Guid eventId)
    {
        throw new NotSupportedException($"Events of type {DailyEventType.PaymentsAndResets} are not stored");
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var @event = new PaymentsAndResetsEvent
        {
            Id = Guid.NewGuid(),
            EventDateTime = DateTime.UtcNow,
        };

        await antiClownApiClient.Economy.UpdateScamCoinsForAllAsync(250, "Ежедневные выплаты");
        await antiClownApiClient.Lohotron.ResetAsync();
        await antiClownApiClient.Shop.ResetAllAsync();

        await dailyEventsMessageProducer.ProduceAsync(@event);

        return @event.Id;
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IDailyEventsMessageProducer dailyEventsMessageProducer;
}