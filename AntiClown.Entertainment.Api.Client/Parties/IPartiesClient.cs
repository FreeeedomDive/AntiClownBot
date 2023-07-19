using AntiClown.Entertainment.Api.Dto.Parties;

namespace AntiClown.Entertainment.Api.Client.Parties;

public interface IPartiesClient
{
    Task<PartyDto> ReadAsync(Guid id);
    Task<PartyDto[]> ReadOpenedAsync();
    Task<Guid> CreateAsync(CreatePartyDto newParty);
    Task UpdateAsync(PartyDto party);
    Task CloseAsync(Guid id);
}