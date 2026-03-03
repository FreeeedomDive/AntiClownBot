using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.Core.Schedules;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;

public class LotteryService : ILotteryService
{
    public LotteryService(
        IAntiClownApiClient antiClownApiClient,
        ICommonEventsRepository commonEventsRepository,
        ICommonEventsMessageProducer commonEventsMessageProducer,
        IAntiClownDataApiClient antiClownDataApiClient,
        IScheduler scheduler
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.commonEventsRepository = commonEventsRepository;
        this.commonEventsMessageProducer = commonEventsMessageProducer;
        this.antiClownDataApiClient = antiClownDataApiClient;
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
        await commonEventsMessageProducer.ProduceAsync(newEvent);
        await ScheduleEventFinishAsync(newEvent.Id);

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
                          .Select(x => antiClownApiClient.Economy.UpdateScamCoinsAsync(x.UserId, new UpdateScamCoinsDto
                          {
                              UserId = x.UserId,
                              ScamCoinsDiff = x.Prize, 
                              Reason = $"Лотерея {eventId}",
                          }));
        await Task.WhenAll(tasks);

        await commonEventsMessageProducer.ProduceAsync(@event);
    }

    private async Task ScheduleEventFinishAsync(Guid eventId)
    {
        var delayInMilliseconds = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.CommonEvents, "LotteryEvent.WaitingTimeInMilliseconds");
        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => SafeFinishAsync(eventId),
                TimeSpan.FromMilliseconds(delayInMilliseconds)
            )
        );
    }

    // keep this method public for Hangfire
    public async Task SafeFinishAsync(Guid eventId)
    {
        try
        {
            await FinishAsync(eventId);
        }
        catch (EventAlreadyFinishedException)
        {
        }
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ICommonEventsMessageProducer commonEventsMessageProducer;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IScheduler scheduler;
}