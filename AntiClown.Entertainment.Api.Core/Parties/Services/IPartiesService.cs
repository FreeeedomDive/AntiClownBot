using AntiClown.Entertainment.Api.Core.Parties.Domain;

namespace AntiClown.Entertainment.Api.Core.Parties.Services;

public interface IPartiesService
{
    Task<Party> ReadAsync(Guid id);
    Task<Party[]> ReadOpenedAsync();
    Task<Guid> CreateAsync(CreateParty newParty);
    Task UpdateAsync(Party party);
    Task CloseAsync(Guid id);
}