using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Entertainment.Api.Core.Parties.Repositories;
using AntiClown.Entertainment.Api.Core.Parties.Services.Messages;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.Parties.Services;

public class PartiesService : IPartiesService
{
    public PartiesService(
        IPartiesRepository partiesRepository,
        IPartiesMessageProducer partiesMessageProducer,
        IScheduler scheduler
    )
    {
        this.partiesRepository = partiesRepository;
        this.partiesMessageProducer = partiesMessageProducer;
        this.scheduler = scheduler;
    }

    public async Task<Party> ReadAsync(Guid id)
    {
        return await partiesRepository.ReadAsync(id);
    }

    public async Task<Party[]> ReadOpenedAsync()
    {
        return await partiesRepository.ReadOpenedAsync();
    }

    public async Task<Guid> CreateAsync(CreateParty newParty)
    {
        var party = new Party
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            IsOpened = true,
            Name = newParty.Name,
            Description = newParty.Description,
            RoleId = newParty.RoleId,
            MaxPlayers = newParty.MaxPlayers,
            Participants = new List<Guid>(),
        };
        var id = await partiesRepository.CreateAsync(party);
        await partiesMessageProducer.ProduceAsync(party);
        ScheduleEventFinish(id);

        return id;
    }

    public async Task UpdateAsync(Party party)
    {
        await partiesRepository.UpdateAsync(party);
        await partiesMessageProducer.ProduceAsync(party);
    }

    public async Task CloseAsync(Guid id)
    {
        var party = await ReadAsync(id);
        if (!party.IsOpened)
        {
            return;
        }

        party.IsOpened = false;
        await UpdateAsync(party);
    }

    private void ScheduleEventFinish(Guid id)
    {
        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => CloseAsync(id),
                TimeSpan.FromDays(2)
            )
        );
    }

    private readonly IPartiesMessageProducer partiesMessageProducer;
    private readonly IPartiesRepository partiesRepository;
    private readonly IScheduler scheduler;
}