using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.Parties.Domain;
using AntiClown.Entertainment.Api.Core.Parties.Repositories;
using AntiClown.Entertainment.Api.Core.Parties.Services.Messages;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.Parties.Services;

public class PartiesService(
    IPartiesRepository partiesRepository,
    IPartiesMessageProducer partiesMessageProducer,
    IScheduler scheduler
) : IPartiesService
{
    public async Task<Party> ReadAsync(Guid id)
    {
        return await partiesRepository.ReadAsync(id);
    }

    public async Task<Party[]> ReadOpenedAsync()
    {
        return await partiesRepository.ReadOpenedAsync();
    }

    public async Task<Party[]> ReadFullPartiesAsync()
    {
        return await partiesRepository.ReadFullPartiesAsync();
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
            CreatorId = newParty.CreatorId,
            FirstFullPartyAt = null,
            Participants = new List<Guid>(),
        };
        if (newParty.AuthorAutoJoin)
        {
            party.Participants.Add(newParty.CreatorId);
        }

        var id = await partiesRepository.CreateAsync(party);
        await partiesMessageProducer.ProduceAsync(party);
        ScheduleEventFinish(id);

        return id;
    }

    public async Task AddPlayerAsync(Guid id, Guid userId)
    {
        var party = await ReadAsync(id);
        if (!party.IsOpened)
        {
            return;
        }

        if (party.Participants.Contains(userId))
        {
            return;
        }

        party.Participants.Add(userId);
        if (party.Participants.Count == party.MaxPlayers && party.FirstFullPartyAt is null)
        {
            party.FirstFullPartyAt = DateTime.UtcNow;
        }

        await UpdateAsync(party);
    }

    public async Task RemovePlayerAsync(Guid id, Guid userId)
    {
        var party = await ReadAsync(id);
        if (!party.IsOpened)
        {
            return;
        }

        if (!party.Participants.Contains(userId))
        {
            return;
        }

        party.Participants.Remove(userId);
        await UpdateAsync(party);
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

    private async Task UpdateAsync(Party party)
    {
        await partiesRepository.UpdateAsync(party);
        await partiesMessageProducer.ProduceAsync(party);
    }

    private void ScheduleEventFinish(Guid id)
    {
        scheduler.Schedule(() => BackgroundJob.Schedule(
                () => CloseAsync(id),
                TimeSpan.FromDays(2)
            )
        );
    }
}