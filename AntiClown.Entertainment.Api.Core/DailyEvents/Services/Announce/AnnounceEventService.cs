using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Domain.Announce;
using AntiClown.Entertainment.Api.Core.DailyEvents.Repositories;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.Messages;
using AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;

namespace AntiClown.Entertainment.Api.Core.DailyEvents.Services.Announce;

public class AnnounceEventService : IAnnounceEventService
{
    public AnnounceEventService(
        IDailyEventsRepository dailyEventsRepository,
        IDailyEventsMessageProducer dailyEventsMessageProducer,
        IAntiClownApiClient antiClownApiClient
    )
    {
        this.dailyEventsRepository = dailyEventsRepository;
        this.dailyEventsMessageProducer = dailyEventsMessageProducer;
        this.antiClownApiClient = antiClownApiClient;
    }

    public async Task<AnnounceEvent> ReadAsync(Guid eventId)
    {
        var @event = await dailyEventsRepository.ReadAsync(eventId);
        if (@event.Type != DailyEventType.Announce)
        {
            throw new WrongEventTypeException($"Event {eventId} is not an announce");
        }

        return (@event as AnnounceEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        // на момент 00:01 по UTC будет еще 19:01 предыдущего дня, поэтому вычитываем транзакции за этот период
        var now = DateTime.UtcNow;
        var transactionsOfTheDay = await antiClownApiClient.Transactions.FindAsync(
            new TransactionsFilterDto
            {
                DateTimeRange = new DateTimeRangeDto
                {
                    From = DateTime.UtcNow.AddDays(-1),
                    To = now,
                },
            }
        );

        var transactionsSumByUser = transactionsOfTheDay
                                    .GroupBy(x => x.UserId)
                                    .ToDictionary(x => x.Key, x => x.Sum(t => t.ScamCoinDiff));

        var announceEvent = new AnnounceEvent
        {
            Id = Guid.NewGuid(),
            EventDateTime = DateTime.UtcNow,
            Earnings = transactionsSumByUser,
        };

        await dailyEventsRepository.CreateAsync(announceEvent);
        await dailyEventsMessageProducer.ProduceAsync(announceEvent);

        return announceEvent.Id;
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IDailyEventsMessageProducer dailyEventsMessageProducer;

    private readonly IDailyEventsRepository dailyEventsRepository;
}