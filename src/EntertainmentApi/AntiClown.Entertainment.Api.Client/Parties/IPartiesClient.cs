/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.Parties;

public interface IPartiesClient
{
    Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto> ReadAsync(System.Guid id);
    Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadOpenedAsync();
    Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadFullAsync();
    Task<System.Guid> CreateAsync(AntiClown.Entertainment.Api.Dto.Parties.CreatePartyDto newParty);
    Task JoinAsync(System.Guid id, System.Guid userId);
    Task LeaveAsync(System.Guid id, System.Guid userId);
    Task CloseAsync(System.Guid id);
}
