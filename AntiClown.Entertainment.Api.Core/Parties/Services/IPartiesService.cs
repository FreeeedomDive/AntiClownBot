using AntiClown.Entertainment.Api.Core.Parties.Domain;

namespace AntiClown.Entertainment.Api.Core.Parties.Services;

public interface IPartiesService
{
    Task<Party> ReadAsync(Guid id);
    Task<Party[]> ReadOpenedAsync();
    Task<Party[]> ReadFullPartiesAsync();
    Task<Guid> CreateAsync(CreateParty newParty);
    Task AddPlayerAsync(Guid id, Guid userId);
    Task RemovePlayerAsync(Guid id, Guid userId);
    Task CloseAsync(Guid id);
}