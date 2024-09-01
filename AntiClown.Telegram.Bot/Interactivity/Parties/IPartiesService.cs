namespace AntiClown.Telegram.Bot.Interactivity.Parties;

public interface IPartiesService
{
    Task CreateOrUpdateMessageAsync(Guid partyId);
}