using AntiClown.Api.Client;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain.PaymentsAndResets;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.Messages;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services.PaymentsAndResets;

public class PaymentsAndResetsService : IPaymentsAndResetsService
{
    public PaymentsAndResetsService(
        IAntiClownApiClient antiClownApiClient,
        IDailyEventsMessageProducer dailyEventsMessageProducer)
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
        await antiClownApiClient.Lohotron.ResetLohotronForAllUsersAsync();
        await ResetAllShopsAsync();

        await dailyEventsMessageProducer.ProduceAsync(@event);

        return @event.Id;
    }

    private async Task ResetAllShopsAsync()
    {
        var users = await antiClownApiClient.Users.ReadAllAsync();
        foreach (var batch in users.Batch(10))
        {
            var tasks = batch.Select(x => antiClownApiClient.Shops.ResetShopAsync(x.Id));
            await Task.WhenAll(tasks);
        }
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IDailyEventsMessageProducer dailyEventsMessageProducer;
}