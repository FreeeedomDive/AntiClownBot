namespace AntiClown.Telegram.Bot.Interactivity.Parties;

public interface IPartiesService
{
    Task CreateOrUpdateMessageAsync(Guid partyId);
    Task JoinPartyAsync(Guid partyId, long userId);
    Task LeavePartyAsync(Guid partyId, long userId);
}