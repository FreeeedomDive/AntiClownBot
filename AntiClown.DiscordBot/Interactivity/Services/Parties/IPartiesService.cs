namespace AntiClown.DiscordBot.Interactivity.Services.Parties;

public interface IPartiesService
{
    Task AddPlayerAsync(Guid partyId, ulong memberId);
    Task RemovePlayerAsync(Guid partyId, ulong memberId);
    Task ClosePartyAsync(Guid partyId, ulong memberId);
    Task CreateOrUpdateAsync(Guid partyId);
}