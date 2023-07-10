using AntiClown.Api.Client;
using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.Common;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;

public class LotteryService : ILotteryService
{
    public LotteryService(
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

    public async Task<LotteryEvent> ReadAsync(Guid eventId)
    {
        var @event = await commonEventsRepository.ReadAsync(eventId);
        if (@event.Type != CommonEventType.Lottery)
        {
            throw new WrongEventTypeException($"Event {eventId} is not a lottery");
        }

        return (@event as LotteryEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var newEvent = LotteryEvent.Create();
        await commonEventsRepository.CreateAsync(newEvent);
        await eventsMessageProducer.ProduceAsync(newEvent);
        ScheduleEventFinish(newEvent.Id);

        return newEvent.Id;
    }

    public async Task AddParticipantAsync(Guid eventId, Guid userId)
    {
        var @event = await ReadAsync(eventId);
        if (@event.Finished)
        {
            throw new EventAlreadyFinishedException(eventId);
        }

        @event.AddParticipant(userId);
        await commonEventsRepository.UpdateAsync(@event);
    }

    public async Task FinishAsync(Guid eventId)
    {
        var @event = await ReadAsync(eventId);
        if (@event.Finished)
        {
            throw new EventAlreadyFinishedException(eventId);
        }

        @event.Finished = true;
        await commonEventsRepository.UpdateAsync(@event);

        var tasks = @event.Participants
            .Select(x => x.Value)
            .Select(x => antiClownApiClient.Economy.UpdateScamCoinsAsync(x.UserId, x.Prize, $"Лотерея {eventId}"));
        await Task.WhenAll(tasks);

        await eventsMessageProducer.ProduceAsync(@event);
    }

    private void ScheduleEventFinish(Guid eventId)
    {
        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => FinishAsync(eventId),
                TimeSpan.FromMilliseconds(Constants.LotteryEventWaitingTimeInMilliseconds))
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IEventsMessageProducer eventsMessageProducer;
    private readonly IScheduler scheduler;
}