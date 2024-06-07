/* Generated file */
namespace AntiClown.Entertainment.Api.Client.Parties;

public interface IPartiesClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto> ReadAsync(System.Guid id);
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadOpenedAsync();
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.Parties.PartyDto[]> ReadFullAsync();
    System.Threading.Tasks.Task<System.Guid> CreateAsync(AntiClown.Entertainment.Api.Dto.Parties.CreatePartyDto newParty);
    System.Threading.Tasks.Task JoinAsync(System.Guid id, System.Guid userId);
    System.Threading.Tasks.Task LeaveAsync(System.Guid id, System.Guid userId);
    System.Threading.Tasks.Task CloseAsync(System.Guid id);
}
