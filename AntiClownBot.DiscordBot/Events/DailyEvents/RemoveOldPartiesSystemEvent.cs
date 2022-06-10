using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Party;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.DailyEvents;

public class RemoveOldPartiesSystemEvent: IDailyEvent
{
    public RemoveOldPartiesSystemEvent(
        IDiscordClientWrapper discordClientWrapper,
        IPartyService partyService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.partyService = partyService;
    }
    
    public Task ExecuteAsync()
    {
        var partiesToRemove = partyService.PartiesInfo
            .OpenParties
            .Where(kv => (DateTime.Now - kv.Value.CreationDate).Days > 0)
            .Select(kv => kv.Key);

        foreach (var partyId in partiesToRemove)
        {
            partyService.RemoveOutdatedParty(partyId);
        }

        partyService.Save();
        
        return Task.CompletedTask;
    }

    public Task<DiscordMessage> TellBackStory()
    {
        return null;
    }

    public bool HasRelatedEvents() => RelatedEvents.Count > 0;

    public List<IEvent> RelatedEvents => new();
    
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IPartyService partyService;
}