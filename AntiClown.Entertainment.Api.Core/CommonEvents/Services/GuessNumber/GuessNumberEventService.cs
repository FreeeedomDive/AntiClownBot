using AntiClown.Api.Client;
using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.Common;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.GuessNumber;

public class GuessNumberEventService : IGuessNumberEventService
{
    public GuessNumberEventService(
        IAntiClownApiClient antiClownApiClient,
        ICommonEventsRepository commonEventsRepository,
        IEventsMessageProducer eventsMessageProducer,
        IScheduler scheduler
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.commonEventsRepository = commonEventsRepository;
        this.eventsMessageProducer = eventsMessageProducer;
        this.scheduler = scheduler;
    }

    public async Task<GuessNumberEvent> ReadAsync(Guid eventId)
    {
        return (await commonEventsRepository.ReadAsync(eventId) as GuessNumberEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var newEvent = new GuessNumberEvent
        {
            Id = Guid.NewGuid(),
            Finished = false,
            EventDateTime = DateTime.UtcNow,
            Picks = new Dictionary<Guid, GuessNumberPick>(),
            NumberToUsers = new Dictionary<GuessNumberPick, List<Guid>>(),
            Result = Enum.GetValues<GuessNumberPick>().SelectRandomItem()
        };
        await commonEventsRepository.CreateAsync(newEvent);
        await eventsMessageProducer.ProduceAsync(newEvent);
        ScheduleEventFinish(newEvent.Id);

        return newEvent.Id;
    }

    public async Task AddParticipantAsync(Guid eventId, Guid userId, GuessNumberPick userPick)
    {
        var @event = await ReadAsync(eventId);
        if (@event.Finished)
        {
            throw new EventAlreadyFinishedException(eventId);
        }

        @event.AddPick(userId, userPick);
        await commonEventsRepository.UpdateAsync(@event);
    }

    public async Task<GuessNumberEvent> FinishAsync(Guid eventId)
    {
        var @event = await ReadAsync(eventId);
        @event.Finished = true;
        await commonEventsRepository.UpdateAsync(@event);

        var winners = @event.NumberToUsers[@event.Result];
        foreach (var winnerUserId in winners)
        {
            await antiClownApiClient.Economy.UpdateLootBoxesAsync(winnerUserId, 1);
        }

        return @event;
    }

    private void ScheduleEventFinish(Guid eventId)
    {
        scheduler.Schedule(() => FinishAsync(eventId),
            TimeSpan.FromMilliseconds(Constants.GuessNumberEventWaitingTimeInMilliseconds));
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IEventsMessageProducer eventsMessageProducer;
    private readonly IScheduler scheduler;
}