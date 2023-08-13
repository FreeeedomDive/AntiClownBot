using AntiClown.Entertainment.Api.Dto.Parties;

namespace AntiClown.Entertainment.Api.Client.Parties;

public interface IPartiesClient
{
    Task<PartyDto> ReadAsync(Guid id);
    Task<PartyDto[]> ReadOpenedAsync();
    Task<PartyDto[]> ReadFullAsync();
    Task<Guid> CreateAsync(CreatePartyDto newParty);
    Task JoinAsync(Guid id, Guid userId);
    Task LeaveAsync(Guid id, Guid userId);
    Task CloseAsync(Guid id);
}