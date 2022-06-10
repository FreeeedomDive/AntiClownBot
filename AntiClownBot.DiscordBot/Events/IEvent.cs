using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events;

public interface IEvent
{
    Task ExecuteAsync();
    Task<DiscordMessage> TellBackStory();
    bool HasRelatedEvents();
    List<IEvent> RelatedEvents { get; }
}