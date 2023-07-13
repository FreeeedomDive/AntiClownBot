namespace AntiClown.Entertainment.Api.Core.DailyEvents.Domain;

public abstract class DailyEventBase
{
    public Guid Id { get; set; }
    public abstract DailyEventType Type { get; }
}