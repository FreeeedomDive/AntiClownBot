using AntiClown.Entertainment.Api.Core.Parties.Domain;

namespace AntiClown.Entertainment.Api.Core.Parties.Repositories;

public interface IPartiesRepository
{
    Task<Party> ReadAsync(Guid id);
    Task<Party[]> ReadOpenedAsync();
    Task<Party[]> ReadFullPartiesAsync();
    Task<Guid> CreateAsync(Party party);
    Task UpdateAsync(Party party);
}