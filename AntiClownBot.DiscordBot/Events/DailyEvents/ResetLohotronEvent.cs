using AntiClownDiscordBotVersion2.Models.Lohotron;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.DailyEvents;

public class ResetLohotronEvent : IDailyEvent
{
    public ResetLohotronEvent(Lohotron lohotron)
    {
        this.lohotron = lohotron;
    }
    
    public Task ExecuteAsync()
    {
        lohotron.Reset();

        return Task.CompletedTask;
    }

    public Task<DiscordMessage> TellBackStory()
    {
        return null;
    }

    public bool HasRelatedEvents() => RelatedEvents.Count > 0;

    public List<IEvent> RelatedEvents => new();
    
    private readonly Lohotron lohotron;
}